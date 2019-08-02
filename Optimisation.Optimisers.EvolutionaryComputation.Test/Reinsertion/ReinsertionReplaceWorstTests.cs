using Optimisation.Base.Management;
using Optimisation.Optimisers.EvolutionaryComputation.Reinsertion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Optimisation.Optimisers.EvolutionaryComputation.Test.Reinsertion
{
    public class ReinsertionReplaceWorstTests
    {
        private readonly Population testPop;

        public ReinsertionReplaceWorstTests()
        {
            var fitnesses = Enumerable.Range(-5, 20).Select(d => (double)d).ToArray();
            var inds = Helpers.CreateEvaluatedIndividualsFromArray(
                fitnesses.Select((f, i) => new[] { 0.5, 1.5 }).ToArray(),
                fitnesses);

            testPop = new Population(initialPopulation: inds);
        }

        [Fact]
        public void ReInsert_WorseThanWorst_DoesNotInsert()
        {
            var newInd = Helpers.CreateEvaluatedIndividualsFromArray(
                new double[][] { new[] { 1.0, 1.0 } },
                new[] { testPop.Worst().Fitness + 1 })
                .First();
            var reinserter = new ReinsertionReplaceWorst();

            reinserter.ReInsert(testPop, newInd);

            Assert.DoesNotContain(newInd, testPop);
        }

        [Fact]
        public void ReInsert_EqualToWorst_DoesNotInsert()
        {
            var newInd = Helpers.CreateEvaluatedIndividualsFromArray(
                new double[][] { new[] { 1.0, 1.0 } }, 
                new[] { testPop.Worst().Fitness })
                .First();
            var reinserter = new ReinsertionReplaceWorst();

            reinserter.ReInsert(testPop, newInd);

            Assert.DoesNotContain(newInd, testPop);
        }

        [Fact]
        public void ReInsert_BetterThanWorst_Inserts()
        {
            var newInd = Helpers.CreateEvaluatedIndividualsFromArray(
                new double[][] { new[] { 1.0, 1.0 } },
                new[] { testPop.Worst().Fitness - 1 })
                .First();
            var reinserter = new ReinsertionReplaceWorst();

            reinserter.ReInsert(testPop, newInd);

            Assert.Contains(newInd, testPop);
        }

    }
}
