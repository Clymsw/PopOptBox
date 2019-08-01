using Optimisation.Base.Management;
using Optimisation.Optimisers.EvolutionaryComputation.Test;
using System;
using System.Linq;
using Xunit;

namespace Optimisation.Optimisers.EvolutionaryComputation.ParentSelection.Test
{
    public class ParentSelectionTournamentTests
    {
        private readonly Population testPop;

        public ParentSelectionTournamentTests()
        {
            var fitnesses = Enumerable.Range(-5, 20).Select(d => (double)d).ToArray();
            var inds = Helpers.CreateEvaluatedIndividualsFromArray(
                fitnesses.Select((f, i) => new[] { 0.5, 1.5 }).ToArray(),
                fitnesses);

            testPop = new Population(initialPopulation: inds);
        }

        [Fact]
        public void Construct_InvalidTournamentSize_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ParentSelectionTournament(0, true));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ParentSelectionTournament(-1, false));
        }

        [Fact]
        public void Select_PopulationSmallerThanTournamentSize_Throws()
        {
            var selector = new ParentSelectionTournament(100);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                selector.Select(testPop, 1));
        }

        [Fact]
        public void Select_NumberToSelectGreaterThanPopulationSize_Throws()
        {
            var selector = new ParentSelectionTournament(10);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                selector.Select(testPop, testPop.Count() + 1));
        }

        [Fact]
        public void Select_RequestOne_GetOne()
        {
            var selector = new ParentSelectionTournament(10, false);
            var parents = selector.Select(testPop, 1);
            Assert.True(parents.Count() == 1);
            Assert.True(parents.ElementAt(0) != testPop.Worst());
        }

        [Fact]
        public void Select_RequestOne_BestIsAlwaysIncluded_GetBest()
        {
            var selector = new ParentSelectionTournament(10, true);
            var parents = selector.Select(testPop, 1);
            Assert.Equal(testPop.Best(), parents.ElementAt(0));
        }

        [Fact]
        public void Select_RequestFive_BestIsAlwaysIncluded_GetFiveIncludingBest()
        {
            var selector = new ParentSelectionTournament(10, true);
            var parents = selector.Select(testPop, 5);
            Assert.True(parents.Count() == 5);
            Assert.Contains(testPop.Best(), parents);
        }
    }
}