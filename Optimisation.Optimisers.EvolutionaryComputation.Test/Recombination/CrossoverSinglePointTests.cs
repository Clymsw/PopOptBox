using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverSinglePointTests
    {
        private readonly DecisionSpace decisionSpaceUniform;
        private readonly DecisionSpace decisionSpaceHetero;
        private readonly DecisionVector parent1;
        private readonly DecisionVector parent2;

        public CrossoverSinglePointTests()
        {
            decisionSpaceUniform = DecisionSpace.CreateForUniformIntArray(
                3, 0, 2);

            parent1 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {0, 1, 2});
            parent2 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {2, 0, 1});

            var heteroSpace = decisionSpaceUniform.Dimensions.ToList();
            heteroSpace.Add(new VariableDiscrete(-1, 1));
            decisionSpaceHetero = new DecisionSpace(heteroSpace);
        }

        [Fact]
        public void Operate_CreatesOnlyExpectedMutations()
        {
            var cx = new CrossoverSinglePoint();
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var child = cx.Operate(parent1, parent2);
                
                Assert.True((int)child.Vector.ElementAt(0) != 1);
                Assert.True((int)child.Vector.ElementAt(1) != 2);
                Assert.True((int)child.Vector.ElementAt(2) != 0);
            }
        }
        
    }
}