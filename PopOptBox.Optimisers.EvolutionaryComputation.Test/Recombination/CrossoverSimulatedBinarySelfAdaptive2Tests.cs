using System;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Optimisers.EvolutionaryComputation.Test;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class CrossoverSimulatedBinarySelfAdaptive2Tests
    {
        private readonly Population population;

        public CrossoverSimulatedBinarySelfAdaptive2Tests()
        {
            var parents = Helpers.CreateFitnessAssessedIndividualsFromArray(
                new[]
                {
                    new[] {2.5, 2.5, 2.5, 2.5},
                    new[] {0.5, 1.5, 2.5, 3.5},
                    new[] {4.5, 3.5, 2.5, 1.5}
                },
                new[] {1.0, 2.0, 3.0});
            
            population = new Population(5, parents);
        }
        
        [Fact]
        public void Operate_VeryHighEta_BestAtCentroid_ReturnsAverageOfParents()
        {
            var cx = new CrossoverSimulatedBinarySelfAdaptive2(population, int.MaxValue - 10);
            var child = cx.Operate(population[1].DecisionVector, population[2].DecisionVector)
                .Select(d => (double)d).ToArray();
            
            // Since we've set eta so high, and the best is located precisely at the centroid,
            // the child should always be located half-way between the two (original) parents.
            Assert.True(child
                    .Select((d,i) => Math.Abs(d - (double)population[0].DecisionVector.ElementAt(i)))
                    .All(d => d < 1e-6));
        }
    }
}