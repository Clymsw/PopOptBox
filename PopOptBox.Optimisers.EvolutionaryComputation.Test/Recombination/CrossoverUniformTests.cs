using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverUniformTests
    {
        private readonly DecisionVector parent1;
        private readonly DecisionVector parent2;
        private readonly DecisionVector parent3Longer;

        public CrossoverUniformTests()
        {
            var decisionSpaceUniform = DecisionSpace.CreateForUniformIntArray(
                4, 0, 3);
            
            parent1 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {0, 1, 2, 3});
            parent2 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {3, 2, 0, 1});
            
            var heteroSpace = decisionSpaceUniform.ToList();
            heteroSpace.Add(new VariableDiscrete(-4, -1));
            heteroSpace.Add(new VariableDiscrete(-4, -1));
            var decisionSpaceHetero = new DecisionSpace(heteroSpace);
            
            parent3Longer = DecisionVector.CreateFromArray(
                decisionSpaceHetero, new[] {1, 2, 3, 0, -1, -2});
        }

        [Fact]
        public void Operate_EqualLengthVectors_CreatesOnlyExpectedCrossovers()
        {
            var cx = new CrossoverUniform();
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var child = cx.Operate(parent1, parent2);
                
                for (var d = 0; d < child.Count; d++)
                {
                    Assert.True(child.ElementAt(d) == parent1.ElementAt(d)
                                || child.ElementAt(d) == parent2.ElementAt(d));
                }
            }
        }
        
        [Fact]
        public void Operate_UnEqualLengthVectors_CreatesOnlyExpectedCrossovers()
        {
            var cx = new CrossoverUniform();
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var child = cx.Operate(parent1, parent3Longer);
                
                Assert.True(child.Count <= parent3Longer.Count
                            && child.Count >= parent1.Count);
                
                for (var d = 0; d < parent1.Count; d++)
                {
                    Assert.True(child.ElementAt(d) == parent1.ElementAt(d)
                                || child.ElementAt(d) == parent3Longer.ElementAt(d));
                }
            }
        }

    }
}