using Optimisation.Base.Variables;
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
            var ds = evaluator.GetGlobalOptimum().GetDecisionSpace();
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds, new[] { 1.0, 1.0 }));
            Assert.True(legal);
        }

        [Fact]
        public void TwoDim_CorrectlyIdentifiesIllegalSolution()
        {
            var evaluator = new Ellipsoidal(2);
            var ds = DecisionSpace.CreateForUniformDoubleArray(2, double.MinValue, double.MaxValue);
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds, new[] { 11.0, -12.0 }));
            Assert.False(legal);
        }

        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0 })]
        [InlineData(new[] { 1.0, 1.0, 3.0 })]
        [InlineData(new[] { 1.0, -1.0, 3.0 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Ellipsoidal(2);
            var ds = evaluator.GetGlobalOptimum().GetDecisionSpace();
            var result = evaluator.Evaluate(DecisionVector.CreateFromArray(ds, new[] { values[0], values[1] }));
            Assert.Equal(values[2], result.ElementAt(0));
        }
    }
}
