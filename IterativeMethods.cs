using System.Reflection.Metadata;

namespace NilsInfinite.NumericalMethods;
public class IterativeMethods
{
    public static double CalculateApproximationError(double exactRoot, double approxRoot)
    {
        return exactRoot - approxRoot;
    }

    public static double CalculateRelativeApproximationError(double exactRoot, double approxRoot)
    {
        return (exactRoot - approxRoot) / exactRoot;
    }

    public static double CalculateFunctionConvergenceDivergence(Func<IEnumerable<Func<double,double>>, double> functions, double tolerance= 1e-6, int maxIterations=200, ErrorTypes errorType = ErrorTypes.Relative)
    {
        var iteration = 0;
        var error = 1.0;
        while(error >= tolerance && iteration <= maxIterations)
        {
            error = errorType switch
            {
                ErrorTypes.Absolute => throw new NotImplementedException(),
                ErrorTypes.Relative => throw new NotImplementedException(),
                _ => throw new InvalidOperationException("Invalid error type provided.")
            };
            iteration++;
        }
        throw new InvalidOperationException("Method did not converge.");
    }
}
