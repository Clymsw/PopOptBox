using System;
using System.Linq;
using MathNet.Numerics.Random;
using Xunit;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Test
{
    public class RandomNumberManagerTests
    {
        private readonly RandomNumberManager rngManager;

        private readonly DecisionVector testDv;
        
        public RandomNumberManagerTests()
        {
            testDv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformIntArray(8, 0, 7),
                new int[8] {7, 6, 5, 4, 3, 2, 1, 0});

            rngManager = new RandomNumberManager(new MersenneTwister(123456789)); 
        }
        
        [Fact]
        public void GetLocations_InvalidNumberOfLocations_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetLocations(testDv.Vector.Count,
                maximumNumberOfLocations: testDv.Vector.Count + 1,
                selectionWithReplacement: false));
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetLocations(testDv.Vector.Count,
                    maximumNumberOfLocations: 0,
                    selectionWithReplacement: false));
        }
        
        [Fact]
        public void GetLocations_InvalidProbabilityOfSelection_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetLocations(testDv.Vector.Count,
                    lambda: -0.01));
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                rngManager.GetLocations(testDv.Vector.Count,
                    lambda: 1.01));
        }
        
        [Fact]
        public void GetLocations_OneLocationRequested_ZeroProbability_ReturnsEmptyList()
        {
            var locations = rngManager.GetLocations(testDv.Vector.Count, 
                maximumNumberOfLocations: 1,
                lambda: 0);
            
            Assert.True(!locations.Any());
        }
        
        [Fact]
        public void GetLocations_OneLocationRequested_CertainProbability_ReturnsOneValidLocation()
        {
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var locations = rngManager.GetLocations(testDv.Vector.Count,
                    maximumNumberOfLocations: 1,
                    selectionWithReplacement: false,
                    lambda: 1);
                
                Assert.True(locations.Count() == 1);
                Assert.True(locations.ElementAt(0) >= 0 && locations.ElementAt(0) < testDv.Vector.Count);
            }
        }
        
        [Fact]
        public void GetLocations_FiveLocationsRequestedWithReplacement_CertainProbability_ReturnsFiveValidLocations()
        {
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var locations = rngManager.GetLocations(testDv.Vector.Count,
                    maximumNumberOfLocations: 5,
                    selectionWithReplacement: true,
                    lambda: 1);
                
                Assert.True(locations.Count() == 5);
                Assert.True(locations.All(l => l >= 0 && l < testDv.Vector.Count));
            }
        }
        
        [Fact]
        public void GetLocations_SameNumberAsLength_NoReplacement_ReturnsAllUniqueValidLocations()
        {
            for (var i = 0; i < 10; i++)
            {
                // Try this a few times to try to cause a mistake.
                var locations = rngManager.GetLocations(testDv.Vector.Count,
                    maximumNumberOfLocations: testDv.Vector.Count,
                    selectionWithReplacement: false,
                    lambda: 1);

                Assert.True(locations.Count() == testDv.Vector.Count);
                Assert.True(locations.Distinct().Count() == testDv.Vector.Count);
                Assert.True(locations.All(l => l >= 0 && l < testDv.Vector.Count));
            }
        }
    }
}