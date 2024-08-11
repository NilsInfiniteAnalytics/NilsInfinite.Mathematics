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
        public void CalculateBisectionMethodRootSearchReturnsCorrectValue()
        {
            // Arrange
            var func = new Func<double, double>(x => x * Math.Sin(x) - 1);
            const double lowerBound = 0.0;
            const double upperBound = 2.0;

            // Act
            var result = IterativeMethods
                .CalculateBisectionMethodRootSearch(func, lowerBound, upperBound);

            // Assert
            Assert.IsNotNull(result);
            var valueTuples = result as (double MidPoint, bool Converged)[] ?? result.ToArray();
            Assert.IsTrue(valueTuples.Last().Converged);
            Assert.AreEqual(1.114157141, valueTuples.Last().MidPoint, 1e-6);
        }

        [TestMethod]
        public void CalculateRegulaFalsiRootSearchReturnsCorrectValue()
        {
            // Arrange
            var func = new Func<double, double>(x => x * Math.Sin(x) - 1);
            const double lowerBound = 0.0;
            const double upperBound = 2.0;

            // Act
            var result = IterativeMethods
                .CalculateRegulaFalsiRootSearch(func, lowerBound, upperBound);

            // Assert
            Assert.IsNotNull(result);
            var valueTuples = result as (double MidPoint, bool Converged)[] ?? result.ToArray();
            Assert.IsTrue(valueTuples.Last().Converged);
            Assert.AreEqual(1.114157141, valueTuples.Last().MidPoint, 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CalculateRegulaFalsiRootSearchThrowsInvalidOperationException()
        {
            // Arrange
            var func = new Func<double, double>(x => 1 / (x - 2));
            const double lowerBound = 1.0;
            const double upperBound = 7.0;

            // Act
            var result = IterativeMethods
                .CalculateRegulaFalsiRootSearch(func, lowerBound, upperBound);

            // Assert
            Assert.IsNotNull(result);
            var valueTuples = result as (double MidPoint, bool Converged)[] ?? result.ToArray();
            Assert.IsTrue(valueTuples.Last().Converged);
            Assert.AreEqual(1.114157141, valueTuples.Last().MidPoint, 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateRegulaFalsiRootSearchThrowsArgumentException()
        {
            // Arrange
            var func = new Func<double, double>(x => 1 / (x - 2));
            const double lowerBound = 8.0;
            const double upperBound = 7.0;

            // Act
            var result = IterativeMethods
                .CalculateRegulaFalsiRootSearch(func, lowerBound, upperBound);

            // Assert
            Assert.IsNotNull(result);
            var valueTuples = result as (double MidPoint, bool Converged)[] ?? result.ToArray();
            Assert.IsTrue(valueTuples.Last().Converged);
            Assert.AreEqual(1.114157141, valueTuples.Last().MidPoint, 1e-6);
        }

        [TestMethod]
        public void CalculateApproximateLocationOfRootsReturnsCorrectValue()
        {
            // Arrange
            var func = new Func<double, double>(x => x * x * x - x * x - x + 1);
            const double lower = -1.2;
            const double upper = 2.0;
            const int numberOfSubIntervals = 1000;

            // Act
            var result = IterativeMethods
                .CalculateApproximateLocationOfRoots(func, lower, upper, numberOfSubIntervals);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(Math.Abs(result.Last().NumberOfRoots - 2) < 1e-6);
        }
    }
}