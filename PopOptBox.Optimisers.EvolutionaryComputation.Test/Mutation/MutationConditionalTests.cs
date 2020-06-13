using System;
using System.Linq;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.EvolutionaryComputation.Test;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class MutationConditionalTests
    {
        private readonly DecisionVector testDv;
        private const int mutation = 2;
        private readonly IMutationOperator insideMutationDoNothing;
        private readonly IMutationOperator insideMutation;

        public MutationConditionalTests()
        {
            testDv = Helpers.CreateDecisionVectorWithMixedElements();
            insideMutationDoNothing = new MutationAddRandomIntegerFromSet(
                new[] { mutation },
                0, 1);
            insideMutation = new MutationAddRandomIntegerFromSet(
                new[] { mutation },
                1, 1);
        }

        [Fact]
        public void Construct_NullConditions_Throws()
        {
            Assert.Throws<NullReferenceException>(() =>
                new MutationConditional(
                    insideMutation, 
                    null));
        }

        [Fact]
        public void Construct_EmptyConditions_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationConditional(
                    insideMutation,
                    new Func<object,bool>[0]));
        }

        [Fact]
        public void Operate_AllConditionsPassed_ZeroMutationChance_ReturnsOriginalDv()
        {
            var mutator = new MutationConditional(
                    insideMutationDoNothing,
                    o => Convert.ToDouble(o) >= double.MinValue);

            var newDv = mutator.Operate(testDv);

            Assert.Equal(testDv, newDv);
        }

        [Fact]
        public void Operate_AllConditionsPassed_OneMutationDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationConditional(
                insideMutation,
                o => Convert.ToDouble(o) >= double.MinValue);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            var changedValue = newDv.Where((v, i) => (double)v != (double)testDv.ElementAt(i)).Select(d => (double)d);
            Assert.True(changedValue.Count() == 1);
            var oldValue = testDv.Where((v, i) => (double)v != (double)newDv.ElementAt(i)).Select(d => (double)d);
            Assert.True(Math.Abs(Math.Abs(oldValue.First() - changedValue.First()) - mutation) < 1e-8);
        }

        [Fact]
        public void Operate_ConditionPassesForOneElement_OneMutationDesired_ReturnsCorrectlyMutatedDv()
        {
            var mutator = new MutationConditional(
                insideMutation,
                o => Convert.ToDouble(o) < 0.01);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            var changedValue = Convert.ToDouble(newDv.Last());
            var oldValue = Convert.ToDouble(testDv.Last());
            Assert.True(Math.Abs(Math.Abs(oldValue - changedValue) - mutation) < 1e-8);
        }

        [Fact]
        public void Operate_MultipleConditionsPassForOneElement_OneMutationDesired_ReturnsCorrectlyMutatedDv()
        {
            var mutator = new MutationConditional(
                insideMutation,
                o => Convert.ToDouble(o) < 1.2,
                o => Convert.ToDouble(o) > 0.8);

            var newDv = mutator.Operate(testDv);

            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            var changedValue = Convert.ToDouble(newDv.ElementAt(newDv.Count - 2));
            var oldValue = Convert.ToDouble(testDv.ElementAt(newDv.Count - 2));
            Assert.True(Math.Abs(Math.Abs(oldValue - changedValue) - mutation) < 1e-8);
        }
    }
}