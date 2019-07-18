using System.Linq;
using Xunit;

namespace Optimisation.Problems.SingleObjective.Continuous.Test
{
    public class EllipsoidalTests
    {
        [Fact]
        public void TwoDim_CorrectlyIdentifiesLegalSolution()
        {
            var evaluator = new Ellipsoidal(2);
            var legal = evaluator.GetLegality(new[] { 1.0, 1.0 });
            Assert.True(legal);
        }

        [Fact]
        public void TwoDim_CorrectlyIdentifiesIllegalSolution()
        {
            var evaluator = new Ellipsoidal(2);
            var legal = evaluator.GetLegality(new[] { 11.0, -12.0 });
            Assert.False(legal);
        }

        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0 })]
        [InlineData(new[] { 1.0, 1.0, 3.0 })]
        [InlineData(new[] { 1.0, -1.0, 3.0 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Ellipsoidal(2);
            var result = evaluator.Evaluate(new[] { values[0], values[1] });
            Assert.Equal(values[2], result.ElementAt(0));
        }
    }
}
