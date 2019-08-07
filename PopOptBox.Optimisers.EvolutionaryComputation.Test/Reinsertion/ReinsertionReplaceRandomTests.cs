﻿using PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion;
using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Test.Reinsertion
{
    public class ReinsertionReplaceRandomTests
    {
        private readonly Population testPop;

        public ReinsertionReplaceRandomTests()
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
        public void ReInsert_WorseThanWorst_DoesNotInsert()
        {
            var newInd = Helpers.CreateEvaluatedIndividualsFromArray(
                new double[][] { new[] { 1.0, 1.0 } },
                new[] { testPop.Worst().Fitness + 1 })
                .First();
            var reinserter = new ReinsertionReplaceRandom();

            reinserter.ReInsert(testPop, newInd);

            Assert.DoesNotContain(newInd, testPop);
        }

        [Fact]
        public void ReInsert_BetterThanBest_Inserts()
        {
            var newInd = Helpers.CreateEvaluatedIndividualsFromArray(
                new double[][] { new[] { 1.0, 1.0 } },
                new[] { testPop.Best().Fitness - 1 })
                .First();
            var reinserter = new ReinsertionReplaceRandom();

            reinserter.ReInsert(testPop, newInd);

            Assert.Contains(newInd, testPop);
        }
    }
}