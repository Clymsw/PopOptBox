using System;
using System.Linq;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.EvolutionaryComputation.Test;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class MutationAddRandomIntegerFromSetTests
    {
        private readonly DecisionVector testDv;

        public MutationAddRandomIntegerFromSetTests()
        {
            testDv = Helpers.CreateDecisionVectorWithMixedElements();
        }

        [Fact]
        public void Construct_InvalidMinMaxValues_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomIntegerFromSet(
                    new[] { 1, 3 },
                    -0.01, 1));
        }

        [Fact]
        public void Construct_InvalidMutationProbability_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomIntegerFromSet(
                    new[] { 1, 3 },
                    -0.01, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomIntegerFromSet(
                    new[] { 1, 3 },
                    1.01, 1));
        }

        [Fact]
        public void Construct_InvalidNumberOfMutations_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationAddRandomIntegerFromSet(
                    new[] { 1, 3 },
                    0.1, 0));
        }

        [Fact]
        public void Operate_OneMutationDesired_ZeroMutationChance_ReturnsOriginalDv()
        {
            var mutator = new MutationAddRandomIntegerFromSet(
                new[] { 1, 3 },
                0, 1);

            var newDv = mutator.Operate(testDv);

            Assert.Equal(testDv, newDv);
        }

        [Fact]
        public void Operate_OneMutationDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationAddRandomIntegerFromSet(
                new[] { 2 },
                1, 1);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            var changedValue = newDv.Where((v, i) => (double)v != (double)testDv.ElementAt(i)).Select(d => (double)d);
            Assert.True(changedValue.Count() == 1);
            var oldValue = testDv.Where((v, i) => (double)v != (double)newDv.ElementAt(i)).Select(d => (double)d);
            Assert.True(Math.Abs(Math.Abs(oldValue.First() - changedValue.First()) - 2.0) < 1e-8);
        }

        [Fact]
        public void Operate_ManyMutationsDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationAddRandomIntegerFromSet(
                new[] { 2, 7 },
                1, 10);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            Assert.True(newDv.Where((v, i) => v == testDv.ElementAt(i)).Count()
                        <= testDv.Count - 1);
        }
    }
}