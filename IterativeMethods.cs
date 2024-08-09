using System.Net;

namespace NilsInfinite.NumericalMethods;
public static class IterativeMethods
{
    public static double Infinitesmal = 1e-6;

    public static double CalculateApproximationError(double exactRoot, double approxRoot)
    {
        return exactRoot - approxRoot;
    }

    public static double CalculateRelativeApproximationError(double exactRoot, double approxRoot)
    {
        return (exactRoot - approxRoot) / exactRoot;
    }

    public static SequenceTypes CalculateFixedPointConvergenceDivergence(Func<double, double> function,
        double initialValue,
        double tolerance = 1e-6,
        int maxIterations = 200,
        ErrorTypes errorType = ErrorTypes.Relative)
    {
        var iteration = 0;
        const double error = 1.0;
        var previousValue = initialValue;
        var currentValue = initialValue;
        var newValue = function(currentValue);
        var dF = newValue - currentValue;
        while (error >= tolerance && iteration <= maxIterations)
        {
            iteration++;
            previousValue = currentValue;
            currentValue = newValue;
            newValue = function(currentValue);
            dF = newValue - currentValue;
            var delta = Math.Abs(dF);
            var relativeError = 2 * delta / (Math.Abs(newValue) + Infinitesmal);
        }
        var dX = currentValue - previousValue;
        var slope = dF / dX;
        return Math.Abs(slope) > 1 ? SequenceTypes.Diverging : SequenceTypes.Converging;
    }

    public static IEnumerable<(double MidPoint, bool Converged)> CalculateBisectionMethodRootSearch(Func<double, double> errorFunc,
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

    public static IEnumerable<(double FalsePosition, bool Converged)> CalculateRegulaFalsiRootSearch(Func<double, double> errorFunc,
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
            if (fFalsePosition == 0)
            {
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
            if (Math.Abs(changeInIterate) < residual && Math.Abs(fFalsePosition) < functionResidual)
            {
                converged = true;
                break;
            }
            iteration++;

        } while (iteration <= maxIterations);
        if (iteration > maxIterations)
        {
            throw new InvalidOperationException("The bisection method did not converge within the maximum number of iterations");
        }
        yield return (falsePosition, converged);
    }

    public static double CalculateBisectionRootSearchMaximumIterations(double lowerBound, double upperBound, double residual = 1e-6)
    {
        return Math.Ceiling((Math.Log(upperBound - lowerBound) - Math.Log(residual)) / Math.Log(2));
    }
}
