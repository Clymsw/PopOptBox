using System;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Problems.SingleObjective.Continuous.Test
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
            var ds = evaluator.GetGlobalOptimum().GetDecisionSpace();
            var result = evaluator.Evaluate(DecisionVector.CreateFromArray(ds, new[] { values[0], values[1] }));
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 0.001);
        }
    }
}
