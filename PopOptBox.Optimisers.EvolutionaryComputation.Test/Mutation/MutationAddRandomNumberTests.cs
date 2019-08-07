using System;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class MutationAddRandomNumberTests
    {
        private readonly DecisionVector invalidDv;
        private readonly DecisionVector testDv;
        
        public MutationAddRandomNumberTests()
        {
            invalidDv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformIntArray(8, 0, 7),
                new int[8] {7, 6, 5, 4, 3, 2, 1, 0});
            
            testDv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(8, double.MinValue, double.MaxValue),
                new double[8] {7, 6, 5, 4, 3, 2, 1, 0});
        }
        
        [Fact]
        public void Construct_InvalidStdDev_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MutationAddRandomNumber(
                -0.01, 0.1, 1));
        }

        [Fact]
        public void Construct_InvalidMutationProbability_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new MutationAddRandomNumber(
                    0.5, -0.01, 1));
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new MutationAddRandomNumber(
                    0.5, 1.01, 1));
        }
        
        [Fact]
        public void Construct_InvalidNumberOfMutations_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new MutationAddRandomNumber(
                    0.5, 0.1, 0));
        }
        
        [Fact]
        public void Operate_DiscreteDv_Throws()
        {
            var mutator = new MutationAddRandomNumber(0.5, 1, 1);
            Assert.Throws<ArgumentException>(() => mutator.Operate(invalidDv));
        }
        
        [Fact]
        public void Operate_OneMutationDesired_ZeroMutationChance_ReturnsOriginalDv()
        {
            var mutator = new MutationAddRandomNumber(
                0.5, 0, 1);

            var newDv = mutator.Operate(testDv);
            
            Assert.Equal(testDv, newDv);
        }
        
        [Fact]
        public void Operate_OneMutationDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationAddRandomNumber(
                    0.5, 1, 1);

            var newDv = mutator.Operate(testDv);
            
            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            Assert.Equal(testDv.Count - 1,
                newDv.Where((v, i) => v == testDv.ElementAt(i)).Count());
        }
        
        [Fact]
        public void Operate_ManyMutationsDesired_ReturnsMutatedDv()
        {
            var mutator = new MutationAddRandomNumber(
                0.5, 1, 10);

            var newDv = mutator.Operate(testDv);
            
            Assert.NotEqual(testDv, newDv);
            Assert.Equal(testDv.Count, newDv.Count);
            Assert.True(newDv.Where((v, i) => v == testDv.ElementAt(i)).Count()
            <= testDv.Count - 1);
        }

    }
}