using System;
using System.Linq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class MutationReplaceWithRandomNumberTests
    {
        private readonly DecisionVector testDv;
        
        public MutationReplaceWithRandomNumberTests()
        {
            testDv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(8, 0, 10),
                new double[8] {7, 6, 5, 4, 3, 2, 1, 0});
        }
        
        [Fact]
        public void Construct_InvalidMutationProbability_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new MutationReplaceWithRandomNumber(
                    -0.01, 1));
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new MutationReplaceWithRandomNumber(
                    1.01, 1));
        }
        
        [Fact]
        public void Construct_InvalidNumberOfMutations_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new MutationReplaceWithRandomNumber(
                    0.1, 0));
        }
        
        [Fact]
        public void Operate_OneMutationDesired_ZeroMutationChance_ReturnsOriginalDv()
        {
            var mutator = new MutationReplaceWithRandomNumber(
                0, 1);

            var newDv = mutator.Operate(testDv);
            
            Assert.Equal(testDv, newDv);
        }
        
        [Fact]
        public void Operate_OneMutationDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationReplaceWithRandomNumber(
                1, 1);

            var newDv = mutator.Operate(testDv);
            
            // TODO: This can fail if it generates the same value it already had!
            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Vector.Count, newDv.Vector.Count);
            Assert.Equal(testDv.Vector.Count - 1,
                newDv.Vector.Where((v, i) => v == testDv.Vector.ElementAt(i)).Count());
        }
        
        [Fact]
        public void Operate_ManyMutationsDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationReplaceWithRandomNumber(
                1, 10);

            var newDv = mutator.Operate(testDv);
            
            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Vector.Count, newDv.Vector.Count);
            Assert.True(newDv.Vector.Where((v, i) => v == testDv.Vector.ElementAt(i)).Count()
                        <= testDv.Vector.Count - 1);
        }
    }
}