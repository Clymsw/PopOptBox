using Xunit;

namespace PopOptBox.Base.FitnessCalculation.Test
{
    public class SolutionToFitnessWeightedSumTests
    {
        [Fact]
        public void CalculateFitness_AllMinimise_ReturnsCorrectOrder()
        {
            var solToFit = new SolutionToFitnessWeightedSum(new[] {1.0, 2.0, 0.5});

            var point1 = new[] { 2.0, 3.0, 10.0 };
            var point2 = new[] { 3.0, 6.0, 5.0 };
            
            Assert.Equal(13.0, solToFit.CalculateFitness(point1));
            Assert.Equal(17.5, solToFit.CalculateFitness(point2));
            Assert.True(solToFit.CalculateFitness(point1) < solToFit.CalculateFitness(point2));
        }
        
        [Fact]
        public void CalculateFitness_MixedMinMax_ReturnsCorrectOrder()
        {
            // Minimise, maximise, minimise
            var solToFit = new SolutionToFitnessWeightedSum(new[] {1.0, -1.0, 1.0});

            var point1 = new[] { 2.0, 3.0, 4.0 }; 
            var point2 = new[] { 2.0, 5.0, 5.0 }; // same, better + 2, worse - 1
            
            Assert.True(solToFit.CalculateFitness(point2) < solToFit.CalculateFitness(point1));
        }
    }
}