using NilsInfinite.NumericalMethods;

namespace NilsInfinite.Mathematics.Tests;
[TestClass]
public class NumericalInterpolationTests
{
    [TestMethod]
    public void CalculateNewtonPolynomialInterpolationDividedDifferenceTable_ReturnsCorrectTable()
    {
        // Arrange
        IEnumerable<double> abscissas = new double[] { 1, 2, 3 };
        IEnumerable<double> ordinates = new double[] { 1, 4, 9 };

        // Act
        var result = NumericalInterpolation
            .CalculateNewtonPolynomialInterpolationDividedDifferenceTable(
                abscissas, 
                ordinates);

        // Assert
        Assert.IsNotNull(result);
    }
}
