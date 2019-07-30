using Optimisation.Base.Management;
using Optimisation.Base.Variables;
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
        public void Construct_InvalidMutationProbability_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationRandomSwap(-0.01));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MutationRandomSwap(1.01));
        }

    }
}