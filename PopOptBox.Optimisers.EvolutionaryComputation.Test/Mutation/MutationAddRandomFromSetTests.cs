using System;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class MutationAddRandomFromSetTests
    {
        private readonly DecisionVector testDv;

        public MutationAddRandomFromSetTests()
        {
            var discreteDs = DecisionSpace.CreateForUniformIntArray(4, int.MinValue, int.MaxValue);
            var continuousDs = DecisionSpace.CreateForUniformDoubleArray(4, double.MinValue, double.MaxValue);
            var variables = discreteDs.ToList();
            variables.AddRange(continuousDs);
            testDv = DecisionVector.CreateFromArray(
                new DecisionSpace(variables),
                new double[8] { 7, 6, 5, 4, 3.1, 2.1, 1.1, 0.0 });
        }

        [Fact]
        public void Construct_InvalidMutationProbability_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomNumberFromSet(
                    new[] { 1.0, 1.2 },
                    -0.01, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomNumberFromSet(
                    new[] { 1.0, 1.2 },
                    1.01, 1));
        }

        [Fact]
        public void Construct_InvalidNumberOfMutations_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomNumberFromSet(
                    new[] { 1.0, 1.2 },
                    0.1, 0));
        }

        [Fact]
        public void Operate_OneMutationDesired_ZeroMutationChance_ReturnsOriginalDv()
        {
            var mutator = new MutationAddRandomNumberFromSet(
                new[] { 1.0, 1.2 },
                0, 1);

            var newDv = mutator.Operate(testDv);

            Assert.Equal(testDv, newDv);
        }

        [Fact]
        public void Operate_OneMutationDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationAddRandomNumberFromSet(
                new[] { 2.0 },
                1, 1);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            var changedValue = newDv.Where((v, i) => (double)v != (double)testDv.ElementAt(i)).Select(d => (double)d);
            Assert.True(changedValue.Count() == 1);
            var oldValue = testDv.Where((v, i) => (double)v != (double)newDv.ElementAt(i)).Select(d => (double)d);
            Assert.Equal(2, Math.Abs(oldValue.First() - changedValue.First()));
        }

        [Fact]
        public void Operate_ManyMutationsDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationAddRandomNumberFromSet(
                new[] { 1.0 },
                1, 10);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            Assert.True(newDv.Where((v, i) => v == testDv.ElementAt(i)).Count()
                        <= testDv.Count - 1);
        }
    }
}