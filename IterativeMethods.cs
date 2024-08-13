using System.Reflection;
using System.Numerics;

namespace NilsInfinite.NumericalMethods;
public static class IterativeMethods
{
    public static double CalculateApproximationError(double exactRoot, double approxRoot)
    {
        return exactRoot - approxRoot;
    }

    public static double CalculateRelativeApproximationError(double exactRoot, double approxRoot)
    {
        return (exactRoot - approxRoot) / exactRoot;
    }

    public static SequenceTypes CalculateFixedPointConvergenceDivergence(
        Func<double, double> errorFunc,
        double initialValue,
        double tolerance = 1e-6,
        int maxIterations = 200,
        ErrorTypes errorType = ErrorTypes.Relative)
    {
        var iteration = 0;
        const double error = 1.0;
        var previousValue = initialValue;
        var currentValue = initialValue;
        var newValue = errorFunc(currentValue);
        var dF = newValue - currentValue;
        while (error >= tolerance && iteration <= maxIterations)
        {
            iteration++;
            previousValue = currentValue;
            currentValue = newValue;
            newValue = errorFunc(currentValue);
            dF = newValue - currentValue;
            var delta = Math.Abs(dF);
            var relativeError = 2 * delta / (Math.Abs(newValue) + 1e-8);
        }
        var dX = currentValue - previousValue;
        var slope = dF / dX;
        return Math.Abs(slope) > 1 ? SequenceTypes.Diverging : SequenceTypes.Converging;
    }

    public static IEnumerable<(double Abscissa, bool Converged)> CalculateBisectionMethodRootSearch(
        Func<double, double> errorFunc,
        double lowerBound,
        double upperBound,
        double residual = 1e-6,
        CancellationToken cancellationToken = default)
    {
        if (lowerBound >= upperBound)
        {
            throw new ArgumentException("The lower bound must be less than the upper bound");
        }
        var iteration = 0;
        var fLower = errorFunc(lowerBound);
        var fUpper = errorFunc(upperBound);
        if (fLower * fUpper > 0)
        {
            throw new ArgumentException("The function does not change sign over the interval");
        }
        var maxIterations = CalculateBisectionRootSearchMaximumIterations(lowerBound, upperBound, residual);
        double midPoint = 0;
        var converged = false;
        while (iteration <= maxIterations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            midPoint = (lowerBound + upperBound) / 2;
            var fMid = errorFunc(midPoint);
            if (Math.Abs(fMid) < residual)
            {
                converged = true;
                break;
            }
            if (fMid * fLower < 0)
            {
                upperBound = midPoint;
            }
            else
            {
                lowerBound = midPoint;
            }
            yield return (midPoint, converged);
            iteration++;
        }
        if (iteration > maxIterations)
        {
            throw new InvalidOperationException("The bisection method did not converge within the maximum number of iterations");
        }
        yield return (midPoint, converged);
    }

    public static IEnumerable<(double Abscissa, bool Converged)> CalculateRegulaFalsiRootSearch(
        Func<double, double> errorFunc,
        double lowerBound,
        double upperBound,
        double residual = 1e-6,
        double functionResidual = 1e-6,
        int maxIterations = 200,
        CancellationToken cancellationToken = default)
    {
        if (lowerBound >= upperBound)
        {
            throw new ArgumentException("The lower bound must be less than the upper bound");
        }
        var iteration = 0;
        var fLower = errorFunc(lowerBound);
        var fUpper = errorFunc(upperBound);
        if (fLower * fUpper > 0)
        {
            throw new ArgumentException("The function does not change sign over the interval");
        }
        double falsePosition;
        var converged = false;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var changeInIterate = fUpper * (upperBound - lowerBound) / (fUpper - fLower);
            falsePosition = upperBound - changeInIterate;
            var fFalsePosition = errorFunc(falsePosition);
            if (Math.Abs(changeInIterate) < residual && Math.Abs(fFalsePosition) < functionResidual)
            {
                converged = true;
                break;
            }
            if (fFalsePosition * fLower < 0)
            {
                upperBound = falsePosition;
                fUpper = fFalsePosition;
            }
            else
            {
                lowerBound = falsePosition;
                fLower = fFalsePosition;
            }
            iteration++;
        } while (iteration <= maxIterations);
        if (iteration > maxIterations)
        {
            throw new InvalidOperationException("The bisection method did not converge within the maximum number of iterations");
        }
        yield return (falsePosition, converged);
    }

    public static IEnumerable<(double NumberOfRoots, double ApproximateRootAbscissas)> CalculateApproximateLocationOfRoots(
        Func<double, double> func,
        double lowerEndPoint,
        double upperEndPoint,
        int numberOfSubIntervals = 100,
        double functionResidual = 1e-2)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func), "The function delegate cannot be null");
        }

        if (lowerEndPoint >= upperEndPoint)
        {
            throw new ArgumentException("The lower end point must be less than the upper end point");
        }

        if (numberOfSubIntervals <= 0)
        {
            throw new ArgumentException("The number of sub-intervals must be greater than zero");
        }

        if (functionResidual <= 0)
        {
            throw new ArgumentException("The function residual must be greater than zero");
        }
        var subIntervalWidth = (upperEndPoint - lowerEndPoint) / numberOfSubIntervals;
        var abscissas = new double[numberOfSubIntervals];
        var ordinates = new double[numberOfSubIntervals];
        var vectorSubIntervalWidth = new Vector<double>(subIntervalWidth);
        var vectorLowerEndPoint = new Vector<double>(lowerEndPoint);
        for (var i = 0; i < numberOfSubIntervals; i += Vector<double>.Count)
        {
            var vectorIndex = new Vector<double>(Enumerable.Range(i, Vector<double>.Count).Select(x => (double)x).ToArray());
            var vectorAbscissas = vectorLowerEndPoint + vectorIndex * vectorSubIntervalWidth;
            for (var j = 0; j < Vector<double>.Count; j++)
            {
                abscissas[i + j] = vectorAbscissas[j];
                ordinates[i + j] = func(abscissas[i + j]);
            }
        }
        var numberOfRoots = 0;
        for (var i = 1; i < numberOfSubIntervals - 1; i++)
        {
            double approximateRootAbscissas;
            if (ordinates[i - 1] * ordinates[i] < 0)
            {
                numberOfRoots++;
                approximateRootAbscissas = (abscissas[i - 1] + abscissas[i]) / 2;
                yield return (numberOfRoots, approximateRootAbscissas);
            }
            var signChangeCheck = (ordinates[i] - ordinates[i - 1]) * (ordinates[i + 1] - ordinates[i]);
            if (!(Math.Abs(ordinates[i]) < functionResidual) || !(signChangeCheck < 0)) continue;
            numberOfRoots++;
            approximateRootAbscissas = (abscissas[i - 1] + abscissas[i]) / 2;
            yield return (numberOfRoots, approximateRootAbscissas);
        }
    }

    public static IEnumerable<(double Abscissa, bool Converged)> CalculateNewtonRaphsonRootSearch(
        Func<double, double> func,
        Func<double, double> derivativeFunc,
        double initialValue,
        double residual = 1e-6,
        int maxIterations = 200,
        CancellationToken cancellationToken = default)
    {
        var iteration = 0;
        var converged = false;
        var currentValue = initialValue;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fValue = func(currentValue);
            var fPrimeValue = derivativeFunc(currentValue);
            if (fPrimeValue == 0)
            {
                throw new DivideByZeroException("The derivative of the function is zero at the current value",
                    innerException: new Exception(MethodBase.GetCurrentMethod()?.Name));
            }
            var newValue = currentValue - fValue / fPrimeValue;
            if (Math.Abs(newValue - currentValue) < residual)
            {
                converged = true;
                break;
            }
            currentValue = newValue;
            yield return (currentValue, converged);
            iteration++;
        } while (iteration <= maxIterations);
        if (iteration > maxIterations)
        {
            throw new InvalidOperationException("The Newton-Raphson method did not converge within the maximum number of iterations",
                innerException: new Exception(MethodBase.GetCurrentMethod()?.Name));
        }
        yield return (currentValue, converged);
    }

    public static IEnumerable<(double Abscissa, bool Converged)> CalculateSecantRootSearch(
        Func<double, double> func,
        double initialValue1,
        double initialValue2,
        double residual = 1e-6,
        int maxIterations = 200,
        CancellationToken cancellationToken = default)
    {
        var iteration = 0;
        var converged = false;
        var previousValue = initialValue1;
        var currentValue = initialValue2;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fPreviousValue = func(previousValue);
            var fCurrentValue = func(currentValue);
            if (fCurrentValue - fPreviousValue == 0)
            {
                throw new DivideByZeroException("The difference between the current and previous values is zero",
                    innerException: new Exception(MethodBase.GetCurrentMethod()?.Name));
            }
            var newValue = currentValue - fCurrentValue * ((currentValue - previousValue) / (fCurrentValue - fPreviousValue));
            if (Math.Abs(newValue - currentValue) < residual)
            {
                converged = true;
                break;
            }
            previousValue = currentValue;
            currentValue = newValue;
            yield return (currentValue, converged);
            iteration++;
        } while (iteration <= maxIterations);
        if (iteration > maxIterations)
        {
            throw new InvalidOperationException("The secant method did not converge within the maximum number of iterations",
                innerException: new Exception(MethodBase.GetCurrentMethod()?.Name));
        }
        yield return (currentValue, converged);
    }

    /// <summary>
    /// Perform Muller's method to find the root of a function using three distinct initial abscissas.
    /// </summary>
    /// <returns>Streams an enumerable of tuples containing the iterate and convergence status</returns>
    /// <exception cref="DivideByZeroException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static IEnumerable<(double Abscissa, bool Converged)> CalculateMullersRootSearch(
        Func<double, double> func,
        double abscissa0,
        double abscissa1,
        double abscissa2,
        double residual = 1e-6,
        double functionResidual = 1e-6,
        int maxIterations = 200,
        CancellationToken cancellationToken = default)
    {
        var iteration = 0;
        var converged = false;
        var fAbscissa0 = func(abscissa0);
        var fAbscissa1 = func(abscissa1);
        var fAbscissa2 = func(abscissa2);
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var abscissaDelta0 = abscissa0 - abscissa2;
            var abscissaDelta1 = abscissa1 - abscissa2;
            var linearSystemEquivalent0 = fAbscissa0 - fAbscissa2;
            var linearSystemEquivalent1 = fAbscissa1 - fAbscissa2;
            var determinant = abscissaDelta0 * abscissaDelta1 * (abscissaDelta0 - abscissaDelta1);
            if (determinant == 0)
            {
                throw new DivideByZeroException("The determinant of the linear system is zero",
                    innerException: new Exception(MethodBase.GetCurrentMethod()?.Name));
            }
            var a = (linearSystemEquivalent0 * abscissaDelta1 - linearSystemEquivalent1 * abscissaDelta0) / determinant;
            var b = (linearSystemEquivalent1 * abscissaDelta0 * abscissaDelta0 - linearSystemEquivalent0 * abscissaDelta1 * abscissaDelta1) / determinant;
            var c = fAbscissa2;
            var discriminant = b * b > 4 * a * c ? Math.Sqrt(b * b - 4 * a * c) : 0.0;
            if (b < 0.0)
            {
                discriminant = -discriminant;
            }
            var z = -2 * c / (b + discriminant);
            var abscissa3 = abscissa2 + z;
            if (Math.Abs(abscissa3 - abscissa1) < Math.Abs(abscissa3 - abscissa0))
            {
                var temp1 = abscissa1;
                var fTemp1 = fAbscissa1;
                abscissa1 = abscissa0;
                abscissa0 = temp1;
                fAbscissa1 = fAbscissa0;
                fAbscissa0 = fTemp1;
            }
            if (Math.Abs(abscissa3 - abscissa2) < Math.Abs(abscissa3 - abscissa1))
            {
                var temp2 = abscissa2;
                var fTemp2 = fAbscissa2;
                abscissa2 = abscissa1;
                abscissa1 = temp2;
                fAbscissa2 = fAbscissa1;
                fAbscissa1 = fTemp2;
            }
            abscissa2 = abscissa3;
            fAbscissa2 = func(abscissa2);
            var relativeError = (2 * Math.Abs(z)) / (Math.Abs(abscissa2) + 1e-8);
            if (relativeError < residual && Math.Abs(fAbscissa2) < functionResidual)
            {
                converged = true;
                break;
            }
            yield return (abscissa2, converged);
            iteration++;
        } while (iteration <= maxIterations);
        if (iteration > maxIterations)
        {
            throw new InvalidOperationException("Muller's method did not converge within the maximum number of iterations",
                innerException: new Exception(MethodBase.GetCurrentMethod()?.Name));
        }
        yield return (abscissa2, converged);
    }

    public static double CalculateBisectionRootSearchMaximumIterations(
        double lowerBound,
        double upperBound,
        double residual = 1e-6)
    {
        return Math.Ceiling((Math.Log(upperBound - lowerBound) - Math.Log(residual)) / Math.Log(2));
    }
}
