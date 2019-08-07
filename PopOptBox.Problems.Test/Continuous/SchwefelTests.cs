using System;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Problems.SingleObjective.Continuous.Test
{
    public class SchwefelTests
    {
        [Fact]
        public void TwoDim_CorrectlyIdentifiesLegalSolution()
        {
            var evaluator = new Schwefel(2);
            var ds = evaluator.GetGlobalOptimum().GetDecisionSpace();
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds, new[] { 1.0, 1.0 }));
            Assert.True(legal);
        }

        [Fact]
        public void TwoDim_CorrectlyIdentifiesIllegalSolution()
        {
            var evaluator = new Schwefel(2);
            var ds = DecisionSpace.CreateForUniformDoubleArray(2, double.MinValue, double.MaxValue);
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds, new[] {-501.0, 210.0 }));
            Assert.False(legal);
            var legal2 = evaluator.GetLegality(DecisionVector.CreateFromArray(ds, new[] {-250.0, 620.0 }));
            Assert.False(legal2);
        }

        [Theory]
        [InlineData(new[] { 420.9687, 420.9687, 0.0 })]
        [InlineData(new[] { 0.0, 0.0, 837.9658 })]
        [InlineData(new[] { 100, -100, 837.9658 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Schwefel(2);
            var ds = evaluator.GetGlobalOptimum().GetDecisionSpace();
            var result = evaluator.Evaluate(DecisionVector.CreateFromArray(ds, new[] { values[0], values[1] }));
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 0.001);
        }
    }
}
