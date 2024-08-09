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
        var maxIterations = Math.Ceiling((Math.Log(upperBound - lowerBound) - Math.Log(residual)) / Math.Log(2));
        double midPoint;
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
        midPoint = (lowerBound + upperBound) / 2;
        yield return (midPoint, converged);
    }
}
