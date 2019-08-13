using Xunit;

namespace PopOptBox.Base.Helpers.Test
{
    public class SolutionToFitnessSingleObjectiveTests
    {
        [Fact]
        public void Maximise_ReturnsValueButNegative()
        {
            var testSolution = new[] { 3.7 };
            Assert.Equal(-1.0 * testSolution[0], SolutionToFitnessSingleObjective.Maximise(testSolution));
        }
        
        [Fact]
        public void Minimise_ReturnsValue()
        {
            var testSolution = new[] { 3.7 };
            Assert.Equal(testSolution[0], SolutionToFitnessSingleObjective.Minimise(testSolution));
        }
    }
}