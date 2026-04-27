using Xunit;
using Krasnoludki.Core;
using System.Collections.Generic;
using System.Linq;

namespace Krasnoludki.Tests
{
    public class ConvexHullSolverTests
    {
        [Fact]
        public void GrahamScan_ShouldReturnCorrectHull_ForSquareWithInnerPoint()
        {
            // Arrange
            var points = new List<Point>
            {
                new Point(0, 0),
                new Point(4, 0),
                new Point(4, 4),
                new Point(0, 4),
                new Point(2, 2) // punkt wewnętrzny
            };

            var expected = new List<Point>
            {
                new Point(0, 0),
                new Point(4, 0),
                new Point(4, 4),
                new Point(0, 4)
            };

            // Act
            var result = ConvexHullSolver.GrahamScan(points);

            // Assert 1: liczba punktów
            Assert.Equal(4, result.Count);

            // Assert 2: czy wynik zawiera dokładnie te punkty (kolejność nieistotna)
            Assert.True(
                expected.All(p => result.Contains(p)) &&
                result.All(p => expected.Contains(p))
            );
        }
    }
}