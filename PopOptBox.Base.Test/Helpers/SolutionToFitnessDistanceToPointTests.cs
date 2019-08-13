using Xunit;

namespace PopOptBox.Base.Helpers.Test
{
    public class SolutionToFitnessDistanceToPointTests
    {
        [Fact]
        public void CalculateFitness_ObjectiveZero_AllMinimise_ReturnsEuclideanDistance()
        {
            var solToFit = new SolutionToFitnessDistanceToPoint(new[] {0.0, 0.0, 0.0});

            var point = new[] { 1.0, 2.0, 2.0 };
            
            Assert.Equal(3.0, solToFit.CalculateFitness(point));
        }
        
        [Fact]
        public void CalculateFitness_ObjectiveZero_MixedMaxMin_ReturnsEuclideanDistance()
        {
            var solToFit = new SolutionToFitnessDistanceToPoint(new[] {0.0, 0.0, 0.0});

            // Maximise, minimise, minimise
            var point = new[] { -10.0, 2.0, 11.0 };
            
            Assert.Equal(15.0, solToFit.CalculateFitness(point));
        }
        
        [Fact]
        public void CalculateFitness_ObjectiveNotZero_MixedMaxMin_ReturnsEuclideanDistance()
        {
            // Maximise, minimise, minimise
            var solToFit = new SolutionToFitnessDistanceToPoint(new[] {10.0, -3.0, 1.0});

            var point = new[] { 3.0, 11.0, 23.0 };
            
            Assert.Equal(27.0, solToFit.CalculateFitness(point));
        }
    }
}