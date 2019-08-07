using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Problems.SingleObjective.Discrete.Test
{
    public class TravellingSalesmanTests
    {
        [Theory]
        [InlineData(new[] { 0.0, 0 }, new[] { 0.0, 3 }, new[] { 4.0, 0 })]
        public void ThreeCitiesInATriangle_CorrectlyCalculatesDistances(params double[][] cityLocations)
        {
            var tsp = new TravellingSalesman("Test Triangle", cityLocations,
                DecisionVector.CreateFromArray(DecisionSpace.CreateForUniformIntArray(4, 0, 2), new[] { 0, 1, 2, 0 }));

            Assert.Equal(12.0, tsp.Evaluate(tsp.GetGlobalOptimum()).ElementAt(0));
        }

        [Fact]
        public void InvalidRoutes_ThrowError()
        {
            var correctDs = DecisionSpace.CreateForUniformIntArray(3, 0, 1);

            var tsp = new TravellingSalesman("A Tale of Two Cities", new List<double[]>() { new[] { 0.0, 0 }, new[] { 1.0, 0 } },
                DecisionVector.CreateFromArray(DecisionSpace.CreateForUniformIntArray(3, 0, 2), new[] { 0, 1, 0 }));

            // Empty route
            Assert.Throws<ArgumentOutOfRangeException>(() => tsp.Evaluate(DecisionVector.CreateForEmpty()));

            // Route that is too long with location (2) that doesn't exist
            Assert.Throws<ArgumentOutOfRangeException>(() => tsp.Evaluate(
                DecisionVector.CreateFromArray(DecisionSpace.CreateForUniformIntArray(4, 0, 2),
                new[] { 0,2,1,0 })));

            // Route with location (2) that doesn't exist
            Assert.Throws<ArgumentOutOfRangeException>(() => tsp.Evaluate(
                DecisionVector.CreateFromArray(correctDs,
                new[] { 0, 2, 0 })));
        }

        [Fact]
        public void ConstructedFromFile_LoadsCorrectly()
        {
            var problemFilePath = Path.Combine(Directory.GetCurrentDirectory(),
                "SingleObjective", "Discrete", "Resources", "ulysses16.tsp");

            var tsp = TravellingSalesman.CreateFromFile(problemFilePath);

            Assert.Equal(17, tsp.GetGlobalOptimum().Vector.Count);
        }
    }
}
