using System;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class MutationRandomSwapTests
    {
        private readonly DecisionVector testDv;

        public MutationRandomSwapTests()
        {
            testDv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(8, double.MinValue, double.MaxValue),
                new double[8] { 7, 6, 5, 4, 3, 2, 1, 0 });
        }

        [Fact]
        public void Construct_InvalidMutationProbability_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationRandomSwap(-0.01));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationRandomSwap(1.01));
        }

        [Fact]
        public void Operate_ZeroMutationChance_ReturnsOriginalDv()
        {
            var mutator = new MutationRandomSwap(0);

            var newDv = mutator.Operate(testDv);

            Assert.Equal(testDv, newDv);
        }

        [Fact]
        public void Operate_CertainMutation_ReturnsMutatedDv()
        {
            var mutator = new MutationRandomSwap(1);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Vector.Count, newDv.Vector.Count);
            Assert.Equal(testDv.Vector.Count - 2,
                newDv.Vector.Where((v, i) => v == testDv.Vector.ElementAt(i)).Count());
        }
    }
}