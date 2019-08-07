using System.Collections.Generic;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class RecombinationParentCentricTests
    {
        private readonly List<DecisionVector> parents;

        public RecombinationParentCentricTests()
        {
            var decisionSpaceUniform = DecisionSpace.CreateForUniformDoubleArray(
                4, 0.0, 5.0);

            parents = new List<DecisionVector>
            {
                DecisionVector.CreateFromArray(
                    decisionSpaceUniform, new[] {0.5, 1.5, 2.5, 3.5}),
                DecisionVector.CreateFromArray(
                    decisionSpaceUniform, new[] {4.5, 3.5, 2.5, 1.5}),
                DecisionVector.CreateFromArray(
                    decisionSpaceUniform, new[] {4.0, 1.0, 4.0, 1.0})
            };
        }
        
        [Fact]
        public void Operate_EqualLengthVectors_ReturnsAverage()
        {
            var pcx = new RecombinationParentCentric();
            var child = pcx.Operate(parents.ToArray());
            // TODO!
        }
    }
}