using System;
using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Optimisers.EvolutionaryComputation.Test;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection.Test
{
    public class ParentSelectionRandomTests
    {
        private readonly Population testPop;

        public ParentSelectionRandomTests()
        {
            var fitnesses = Enumerable.Range(-5, 20).Select(d => (double)d).ToArray();
            var inds = Helpers.CreateFitnessAssessedIndividualsFromArray(
                fitnesses.Select((f, i) => new[] { 0.5, 1.5 }).ToArray(),
                fitnesses);

            testPop = new Population(initialPopulation: inds);
        }

        [Fact]
        public void Select_NumberToSelectGreaterThanPopulationSize_Throws()
        {
            var selector = new ParentSelectionRandom();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                selector.Select(testPop, testPop.Count() + 1));
        }

        [Fact]
        public void Select_RequestOne_GetOne()
        {
            var selector = new ParentSelectionRandom();
            var parents = selector.Select(testPop, 1);
            Assert.True(parents.Count() == 1);
            Assert.Contains(parents.ElementAt(0), testPop);
        }

        [Fact]
        public void Select_RequestFive_GetFive()
        {
            var selector = new ParentSelectionRandom();
            var parents = selector.Select(testPop, 5);
            Assert.True(parents.Count() == 5);
        }
    }
}