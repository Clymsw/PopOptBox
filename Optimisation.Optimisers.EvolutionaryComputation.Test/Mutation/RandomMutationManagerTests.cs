using System;
using System.Linq;
using MathNet.Numerics.Random;
using Xunit;
using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation.Test
{
    public class RandomMutationManagerTests
    {
        private readonly RandomNumberManager rngManager;

        private readonly DecisionVector testDv;
        
        public RandomMutationManagerTests()
        {
            testDv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformIntArray(8, 0, 7),
                new int[8] {7, 6, 5, 4, 3, 2, 1, 0});

            rngManager = new RandomNumberManager(new MersenneTwister(123456789)); 
        }
        
        [Fact]
        public void GetMutationLocations_InvalidNumberOfMutations_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetMutationLocations(testDv,
                maximumNumberOfMutations: testDv.Vector.Count + 1,
                mutationWithReplacement: false));
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetMutationLocations(testDv,
                    maximumNumberOfMutations: 0,
                    mutationWithReplacement: false));
        }
        
        [Fact]
        public void GetMutationLocations_InvalidProbabilityOfMutation_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetMutationLocations(testDv,
                    lambda: -0.01));
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetMutationLocations(testDv,
                    lambda: 1.01));
        }
        
        [Fact]
        public void GetMutationLocations_OneMutationRequested_ZeroProbabilityOfMutation_ReturnsEmptyList()
        {
            var locations = rngManager.GetMutationLocations(testDv, 
                maximumNumberOfMutations: 1,
                lambda: 0);
            
            Assert.True(!locations.Any());
        }
        
        [Fact]
        public void GetMutationLocations_OneMutationRequested_OneMutationOccurs_ReturnsOneValidLocation()
        {
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var locations = rngManager.GetMutationLocations(testDv,
                    maximumNumberOfMutations: 1,
                    mutationWithReplacement: false,
                    lambda: 1);
                
                Assert.True(locations.Count() == 1);
                Assert.True(locations.ElementAt(0) >= 0 && locations.ElementAt(0) < testDv.Vector.Count);
            }
        }
        
        [Fact]
        public void GetMutationLocations_FiveMutationsRequestedWithReplacement_FiveMutationsOccur_ReturnsFiveValidLocations()
        {
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var locations = rngManager.GetMutationLocations(testDv,
                    maximumNumberOfMutations: 5,
                    mutationWithReplacement: true,
                    lambda: 1);
                
                Assert.True(locations.Count() == 5);
                Assert.True(locations.All(l => l >= 0 && l < testDv.Vector.Count));
            }
        }
        
        [Fact]
        public void GetMutationLocations_AllMutationsRequestedWithoutReplacement_ReturnsAllUniqueValidLocations()
        {
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var locations = rngManager.GetMutationLocations(testDv,
                    maximumNumberOfMutations: testDv.Vector.Count,
                    mutationWithReplacement: false,
                    lambda: 1);

                Assert.True(locations.Count() == testDv.Vector.Count);
                Assert.True(locations.Distinct().Count() == testDv.Vector.Count);
                Assert.True(locations.All(l => l >= 0 && l < testDv.Vector.Count));
            }
        }
    }
}