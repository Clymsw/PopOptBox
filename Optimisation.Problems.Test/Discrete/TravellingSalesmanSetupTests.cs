using System.IO;
using System.Linq;
using Xunit;

namespace Optimisation.Problems.SingleObjective.Discrete.Test
{
    public class TravellingSalesmanSetupTests
    {
        private readonly string solutionFilePath;

        public TravellingSalesmanSetupTests()
        {
            solutionFilePath = Path.Combine(Directory.GetCurrentDirectory(),
                "SingleObjective", "Discrete", "Resources", "ulysses16.tsp");
        }

        [Fact]
        public void ConstructedWithUlysses16_LoadsCorrectly()
        {
            var tspBuilder = new TravellingSalesmanSetup(solutionFilePath);

            Assert.Equal(16, tspBuilder.Locations.Count);
            Assert.True(tspBuilder.OptimumRoute.All(l => l >= 0 && l < tspBuilder.Locations.Count));
            Assert.Equal(tspBuilder.OptimumRoute.First(), tspBuilder.OptimumRoute.Last());
        }

    }
}
