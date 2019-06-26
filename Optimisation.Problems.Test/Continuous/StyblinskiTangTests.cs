using System;
using System.Linq;
using Xunit;

namespace Optimisation.Problems.Continuous.SingleObjective.Test
{
    public class StyblinskiTangTests
    {
        [Fact]
        public void TwoDim_CorrectlyIdentifiesLegalSolution()
        {
            var evaluator = new StyblinskiTang(2);
            var legal = evaluator.GetLegality(new[] { 1.0, 1.0 });
            Assert.True(legal);
        }

        [Fact]
        public void TwoDim_CorrectlyIdentifiesIllegalSolutions()
        {
            var evaluator = new StyblinskiTang(2);
            var legal = evaluator.GetLegality(new[] { -6.0, 2.0 });
            Assert.False(legal);
            var legal2 = evaluator.GetLegality(new[] { -1.0, 7.0 });
            Assert.False(legal2);
        }

        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0 })]
        [InlineData(new[] { -2.903534, -2.903534, -78.332 })]
        [InlineData(new[] { 1.0, -1.0, -15.0 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new StyblinskiTang(2);
            var result = evaluator.Evaluate(new[] { values[0], values[1] });
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 0.001);
        }
    }
}
