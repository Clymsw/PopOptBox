using System;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverArithmeticWeightedTests
    {
        private readonly DecisionVector parent1;
        private readonly DecisionVector parent2;
        private readonly DecisionVector parent3Bad;

        public CrossoverArithmeticWeightedTests()
        {
            var decisionSpaceUniform = DecisionSpace.CreateForUniformDoubleArray(
                4, 0.0, 5.0);

            parent1 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {0.5, 1.5, 2.5, 3.5});
            parent2 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {4.5, 3.5, 2.5, 1.5});
            
            var heteroSpace = decisionSpaceUniform.ToList();
            heteroSpace.RemoveAt(0);
            heteroSpace.Add(new VariableDiscrete(1, 4));
            var decisionSpaceHetero = new DecisionSpace(heteroSpace);
            
            parent3Bad = DecisionVector.CreateFromArray(
                decisionSpaceHetero, new[] {2.0, 2.0, 2.0, 2});
        }
        
        [Fact]
        public void Operate_EqualLengthVectors_BiasedToFirstParent_ReturnsFirstParent()
        {
            var cx = new CrossoverArithmeticWeighted(fixedWeight: 1);
            var child = cx.Operate(parent1, parent2);
            Assert.Equal(parent1, child);
        }
        
        [Fact]
        public void Operate_EqualLengthVectors_BiasedToSecondParent_ReturnsSecondParent()
        {
            var cx = new CrossoverArithmeticWeighted(fixedWeight: 0);
            var child = cx.Operate(parent1, parent2);
            Assert.Equal(parent2, child);
        }
        
        [Fact]
        public void Operate_EqualLengthVectors_NoBias_ReturnsAverage()
        {
            var cx = new CrossoverArithmeticWeighted();
            var child = cx.Operate(parent1, parent2);
            for (var i = 0; i < child.Count; i++)
            {
                var avg = 0.5 * ((double) parent1.ElementAt(i) + (double) parent2.ElementAt(i));
                Assert.True(Math.Abs((double) child.ElementAt(i) - avg) < 1e-6);
            }
        }
        
        [Fact]
        public void Operate_DifferentLengthVectors_Throws()
        {
            var cx = new CrossoverArithmeticWeighted();
            Assert.Throws<ArgumentOutOfRangeException>(() => cx.Operate(parent1, parent3Bad));
        }
    }
}