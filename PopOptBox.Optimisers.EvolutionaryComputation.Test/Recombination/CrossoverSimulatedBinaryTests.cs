using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverSimulatedBinaryTests
    {
        private readonly DecisionVector parent1;
        private readonly DecisionVector parent2;

        public CrossoverSimulatedBinaryTests()
        {
            var decisionSpaceUniform = DecisionSpace.CreateForUniformDoubleArray(
                4, 0.0, 5.0);

            parent1 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {0.5, 1.5, 2.5, 3.5});
            parent2 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {4.5, 3.5, 2.5, 1.5});
        }
        
        [Fact]
        public void Operate_EqualLengthVectors_NoBias_ReturnsAverage()
        {
            var cx = new CrossoverSimulatedBinary(2);
            var child = cx.Operate(parent1, parent2);
            // TODO!
        }
    }
}