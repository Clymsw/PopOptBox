using System.Linq;
using Xunit;

namespace Optimisation.Problems.Continuous.SingleObjective.Test
{
    public class RosenbrockTests
    {
        [Theory]
        [InlineData(new[] { 0.0, 0.0, 1.0 })]
        [InlineData(new[] { 1.0, 1.0, 0.0 })]
        [InlineData(new[] { 1.0, -1.0, 400.0 })]
        public void TwoDim_EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Rosenbrock(2);
            var result = evaluator.Evaluate(new[] { values[0], values[1] });
            Assert.Equal(values[2], result.ElementAt(0));
        }
    }
}
