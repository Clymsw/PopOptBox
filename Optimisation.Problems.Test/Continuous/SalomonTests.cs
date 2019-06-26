using System;
using System.Linq;
using Xunit;

namespace Optimisation.Problems.Continuous.SingleObjective.Test
{
    public class SalomonTests
    {
        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0 })]
        [InlineData(new[] { 1.0, 1.0, 2.0 })]
        [InlineData(new[] { 1.0, -1.0, 2.0 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Salomon(2);
            var result = evaluator.Evaluate(new[] { values[0], values[1] });
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 0.001);
        }
    }
}
