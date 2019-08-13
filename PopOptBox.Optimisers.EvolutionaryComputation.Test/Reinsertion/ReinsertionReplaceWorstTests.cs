using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Optimisers.EvolutionaryComputation.Reinsertion;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Test.Reinsertion
{
    public class ReinsertionReplaceWorstTests
    {
        private readonly Population testPop;

        public ReinsertionReplaceWorstTests()
        {
            var fitnesses = Enumerable.Range(-5, 20).Select(d => (double)d).ToArray();
            var inds = Helpers.CreateFitnessAssessedIndividualsFromArray(
                fitnesses.Select((f, i) => new[] { 0.5, 1.5 }).ToArray(),
                fitnesses);

            testPop = new Population(initialPopulation: inds);
        }

        [Fact]
        public void ReInsert_WorseThanWorst_DoesNotInsert()
        {
            var newInd = Helpers.CreateFitnessAssessedIndividualsFromArray(
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
            var newInd = Helpers.CreateFitnessAssessedIndividualsFromArray(
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
            var newInd = Helpers.CreateFitnessAssessedIndividualsFromArray(
                new double[][] { new[] { 1.0, 1.0 } },
                new[] { testPop.Worst().Fitness - 1 })
                .First();
            var reinserter = new ReinsertionReplaceWorst();

            reinserter.ReInsert(testPop, newInd);

            Assert.Contains(newInd, testPop);
        }
    }
}
