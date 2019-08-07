using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverArithmeticMultiParentTests
    {
        private readonly List<DecisionVector> parents;
        private readonly DecisionVector parentBad;

        public CrossoverArithmeticMultiParentTests()
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

            var heteroSpace = decisionSpaceUniform.ToList();
            heteroSpace.RemoveAt(0);
            heteroSpace.Add(new VariableDiscrete(1, 4));
            var decisionSpaceHetero = new DecisionSpace(heteroSpace);
            
            parentBad = DecisionVector.CreateFromArray(
                decisionSpaceHetero, new[] {2.0, 2.0, 2.0, 2});
        }
        
        [Fact]
        public void Operate_EqualLengthVectors_ReturnsAverage()
        {
            var cx = new CrossoverArithmeticMultiParent();
            var child = cx.Operate(parents.ToArray());
            Assert.Equal(
                new[] {3.0, 2.0, 3.0, 2.0},
                child.Select(d => (double) d));
        }
        
        [Fact]
        public void Operate_DifferentLengthVectors_Throws()
        {
            parents.Add(parentBad);
            var cx = new CrossoverArithmeticMultiParent();
            Assert.Throws<ArgumentOutOfRangeException>(() => cx.Operate(parents.ToArray()));
        }
    }
}