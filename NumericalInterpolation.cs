using System.Numerics;

namespace NilsInfinite.NumericalMethods;
public static class NumericalInterpolation
{
    public static double[,] CalculateNewtonPolynomialInterpolationDividedDifferenceTable(
        IEnumerable<double> abscissas,
        IEnumerable<double> ordinates)
    {
        if (abscissas is null) throw new ArgumentNullException(nameof(abscissas));
        if (ordinates is null) throw new ArgumentNullException(nameof(ordinates));
        var enumerableAbscissas = abscissas as double[] ?? abscissas.ToArray();
        var enumerableOrdinates = ordinates as double[] ?? ordinates.ToArray();
        if (enumerableAbscissas.Length != enumerableOrdinates.Length)
        {
            throw new ArgumentException("Abscissas and ordinates must have the same number of elements.");
        }
        var degreeOfPolynomial = enumerableAbscissas.Length - 1;
        var dividedDifferenceTable = new double[degreeOfPolynomial + 1, degreeOfPolynomial + 1];
        for (var i = 0; i <= degreeOfPolynomial; i++)
        {
            dividedDifferenceTable[i, 0] = enumerableOrdinates[i];
        }
        for (var j = 1; j <= degreeOfPolynomial; j++)
        {
            for (var i = 0; i <= degreeOfPolynomial - j; i++)
            {
                var abscissaDelta = enumerableAbscissas[i + j] - enumerableAbscissas[i];
                if (Math.Abs(abscissaDelta) < double.Epsilon || abscissaDelta == 0)
                {
                    throw new ArgumentException("Abscissas must be distinct.");
                }
                dividedDifferenceTable[i, j] = (dividedDifferenceTable[i, j - 1] - dividedDifferenceTable[i - 1, j - 1])
                                                    / abscissaDelta;
            }
        }
        return dividedDifferenceTable;
    }
}
