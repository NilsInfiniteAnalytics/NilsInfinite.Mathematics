using NilsInfinite.NumericalMethods;

namespace NilsInfinite.Mathematics.Tests
{
    [TestClass]
    public class IterativeMethodsTests
    {
        [TestMethod]
        public void TestCalculateFixedPointConvergenceDivergence()
        {
            // Arrange
            var func = new Func<double, double>(x => Math.Pow(x, 2) - 2);
            const double initialValue = 1.9;

            // Act
            var result = IterativeMethods.CalculateFixedPointConvergenceDivergence(func, initialValue);

            // Assert
            Assert.AreEqual(SequenceTypes.Converging, result);
        }

        [TestMethod]
        public void TestCalculateBisectionMethodRootSearch()
        {
            // Arrange
            var func = new Func<double, double>(x => x * Math.Sin(x) - 1);
            const double lowerBound = 0.0;
            const double upperBound = 2.0;

            // Act
            var result = IterativeMethods.CalculateBisectionMethodRootSearch(func, lowerBound, upperBound);

            // Assert
            Assert.IsNotNull(result);
            var valueTuples = result as (double MidPoint, bool Converged)[] ?? result.ToArray();
            Assert.IsTrue(valueTuples.Last().Converged);
            Assert.AreEqual(1.114157141, valueTuples.Last().MidPoint, 1e-6);
        }

    }
}