using System;
using System.Linq;
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
                4, double.MinValue, double.MaxValue);

            parent1 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {0.5, 1.5, 2.5, 3.5});
            parent2 = DecisionVector.CreateFromArray(
                decisionSpaceUniform, new[] {4.5, 3.5, 2.5, 1.5});
        }
        
        [Fact]
        public void Operate_VeryHighEta_ReturnsOneOrOtherParent()
        {
            var cx = new CrossoverSimulatedBinary(int.MaxValue - 10);
            var child = cx.Operate(parent1, parent2)
                .Select(d => (double)d).ToArray();
            
            // Since we've set eta so high, the child should always be very close to one or other parent.
            Assert.True(
                child
                    .Select((d,i) => Math.Abs(d - (double)parent1.ElementAt(i)))
                    .All(d => d < 1e-6) 
                || child
                    .Select((d,i) => Math.Abs(d - (double)parent2.ElementAt(i)))
                    .All(d => d < 1e-6));
        }
    }
}