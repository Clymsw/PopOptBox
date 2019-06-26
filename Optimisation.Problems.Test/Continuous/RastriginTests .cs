using System;
using System.Linq;
using Xunit;

namespace Optimisation.Problems.Continuous.SingleObjective.Test
{
    public class RastriginTests
    {
        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0 })]
        [InlineData(new[] { 1 / (6 * Math.PI), 1 / (6 * Math.PI), 1.1065 })]
        [InlineData(new[] { 1 / (6 * Math.PI), -1 / (6 * Math.PI), 1.1065 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Rastrigin(2);
            var result = evaluator.Evaluate(new[] { values[0], values[1] });
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 0.001);
        }
    }
}
