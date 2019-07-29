using System.Linq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverSinglePointTests
    {
        private readonly DecisionVector parent1;
        private readonly DecisionVector parent2;
        private readonly DecisionVector parent3Longer;

        public CrossoverSinglePointTests()
        {
            var decisionSpaceUniform = DecisionSpace.CreateForUniformIntArray(
                4, 0, 3);
            
            parent1 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {0, 1, 2, 3});
            parent2 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {3, 2, 0, 1});

            var heteroSpace = decisionSpaceUniform.Dimensions.ToList();
            heteroSpace.Add(new VariableDiscrete(-4, -1));
            var decisionSpaceHetero = new DecisionSpace(heteroSpace);

            parent3Longer = DecisionVector.CreateFromArray(
                decisionSpaceHetero, new[] {1, 2, 3, 0, -3});
        }

        [Fact]
        public void Operate_EqualLengthVectors_CreatesOnlyExpectedCrossovers()
        {
            var cx = new CrossoverSinglePoint();
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var child = cx.Operate(parent1, parent2);
                
                for (var d = 0; d < child.Vector.Count; d++)
                {
                    Assert.True(child.Vector.ElementAt(d) == parent1.Vector.ElementAt(d)
                                || child.Vector.ElementAt(d) == parent2.Vector.ElementAt(d));
                }
            }
        }
        
        [Fact]
        public void Operate_UnEqualLengthVectors_CreatesOnlyExpectedCrossovers()
        {
            var cx = new CrossoverSinglePoint();
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var child = cx.Operate(parent1, parent3Longer);
                
                Assert.True(child.Vector.Count == parent3Longer.Vector.Count
                            || child.Vector.Count == parent1.Vector.Count);
                
                for (var d = 0; d < parent1.Vector.Count; d++)
                {
                    Assert.True(child.Vector.ElementAt(d) == parent1.Vector.ElementAt(d)
                                || child.Vector.ElementAt(d) == parent3Longer.Vector.ElementAt(d));
                }
            }
        }
    }
}