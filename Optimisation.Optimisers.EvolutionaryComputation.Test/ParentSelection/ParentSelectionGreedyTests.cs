using Optimisation.Base.Management;
using Optimisation.Optimisers.EvolutionaryComputation.Test;
using System;
using System.Linq;
using Xunit;

namespace Optimisation.Optimisers.EvolutionaryComputation.ParentSelection.Test
{
    public class ParentSelectionGreedyTests
    {
        private readonly Population testPop;

        public ParentSelectionGreedyTests()
        {
            var fitnesses = Enumerable.Range(-5, 20).Select(d => (double)d).ToArray();
            var inds = Helpers.CreateEvaluatedIndividualsFromArray(
                fitnesses.Select((f, i) => new[] { 0.5, 1.5 }).ToArray(),
                fitnesses);

            testPop = new Population(initialPopulation: inds);
        }

        [Fact]
        public void Select_NumberToSelectGreaterThanPopulationSize_Throws()
        {
            var selector = new ParentSelectionGreedy();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                selector.Select(testPop, testPop.Count() + 1));
        }

        [Fact]
        public void Select_RequestOne_GetBest()
        {
            var selector = new ParentSelectionGreedy();
            var parents = selector.Select(testPop, 1);
            Assert.True(parents.Count() == 1);
            Assert.Equal(testPop.Best(), parents.ElementAt(0));
        }

        [Fact]
        public void Select_RequestFive_GetFiveBest()
        {
            var selector = new ParentSelectionGreedy();
            var parents = selector.Select(testPop, 5);
            Assert.True(parents.Count() == 5);
            for (var i = 0; i < 5; i++)
            {
                Assert.Equal(testPop[i], parents.ElementAt(i));
            }
        }
    }
}