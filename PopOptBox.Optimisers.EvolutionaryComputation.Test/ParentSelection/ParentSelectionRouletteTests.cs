using System;
using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Optimisers.EvolutionaryComputation.Test;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection.Test
{
    public class ParentSelectionRouletteTests
    {
        private readonly Population testPop;

        public ParentSelectionRouletteTests()
        {
            var fitnesses = Enumerable.Range(-5, 20).Select(d => (double)d).ToArray();
            var inds = Helpers.CreateEvaluatedIndividualsFromArray(
                fitnesses.Select((f, i) => new[] { 0.5, 1.5 }).ToArray(),
                fitnesses);

            testPop = new Population(
                SolutionToFitness.SingleObjectiveMinimise, Penalty.DeathPenalty,
                initialPopulation: inds);
        }

        [Fact]
        public void Select_NumberToSelectGreaterThanPopulationSize_Throws()
        {
            var selector = new ParentSelectionRoulette(false);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                selector.Select(testPop, testPop.Count() + 1));
        }

        [Fact]
        public void Select_RequestOne_BestAlwaysReturned_GetBest()
        {
            var selector = new ParentSelectionRoulette(true);
            var parents = selector.Select(testPop, 1);
            Assert.True(parents.Count() == 1);
            Assert.Equal(testPop.Best(), parents.ElementAt(0));
        }

        [Fact]
        public void Select_RequestFive_BestAlwaysReturned_GetFiveIncludingBest()
        {
            var selector = new ParentSelectionRoulette(true);
            var parents = selector.Select(testPop, 5);
            Assert.True(parents.Count() == 5);
            Assert.Contains(testPop.Best(), parents);
        }
    }
}