using System;
using System.Linq;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class PopulationTests
    {
        private readonly Individual unevaluatedInd;
        private readonly Individual ind;
        
        private readonly Population popConstLenDv;
        private readonly Population popConstLenDvInitInd;
        private readonly Population popVarLenDv;
        
        public PopulationTests()
        {
            popConstLenDv = new Population(140, null, true);
            
            var testVector = new[] { 0.5, 1.5 };
            
            unevaluatedInd = ObjectCreators.GetIndividual(testVector);

            var fitness = 2;
            
            ind = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(testVector), fitness);
            
            popConstLenDvInitInd = new Population(140, new[] {ind}, true);
            popVarLenDv = new Population(140, new[] {ind}, false);
        }

        [Fact]
        public void NewEmptyPopulation_MembersListIsInitialisedEmpty()
        {
            Assert.True(popConstLenDv.Count == 0);
            Assert.Empty(popConstLenDv.GetMemberFitnesses());
        }
        
        [Fact]
        public void NewEmptyPopulation_BestAndWorstThrowError()
        {
            Assert.Throws<System.InvalidOperationException>(
                () => popConstLenDv.Best());
            Assert.Throws<System.InvalidOperationException>(
                () => popConstLenDv.Worst());
            Assert.Throws<System.InvalidOperationException>(
                () => popConstLenDv.ReplaceWorst(ind));
        }
        
        [Fact]
        public void NewPopulationWithInd_MembersListIsInitialised()
        {
            Assert.True(popConstLenDvInitInd.Count == 1);
            Assert.NotEmpty(popConstLenDvInitInd.GetMemberFitnesses());
            Assert.Equal(ind, popConstLenDvInitInd[0]);
        }
        
        [Fact]
        public void NewEmptyPopulation_AddUnevaluatedIndividual_Fails()
        {
            Assert.Throws<ArgumentException>(
                () => popConstLenDv.AddIndividual(unevaluatedInd));
        }
        
        [Fact]
        public void NewEmptyPopulation_AddFirstIndividual_InsertedOk()
        {
            popConstLenDv.AddIndividual(ind);
            
            Assert.True(popConstLenDv.Count == 1);
            Assert.Equal(popConstLenDv[0], ind);
        }

        [Fact]
        public void ConstantLengthDvPop_AddIndividualWithDifferentLengthDv_Fails()
        {
            var numDims = ind.DecisionVector.Vector.Count;
            var ind1 = ObjectCreators.GetIndividual(Enumerable.Range(0, numDims + 1).ToArray());
            
            Assert.Throws<ArgumentException>(
                () => popConstLenDvInitInd.AddIndividual(ind1));
        }
        
        [Fact]
        public void VariableLengthDvPop_AddIndividualWithDifferentLengthDv_Succeeds()
        {
            var numDims = ind.DecisionVector.Vector.Count;
            var ind1 = ObjectCreators.EvaluateIndividual( 
                ObjectCreators.GetIndividual(Enumerable.Range(0, numDims + 1).ToArray()));
            
            popVarLenDv.AddIndividual(ind1);
            
            Assert.True(popVarLenDv.Count == 2);
            Assert.Equal(popVarLenDv[1], ind1);
        }

        [Fact]
        public void ConstantLengthDvPop_AddWorseIndividual_SortsCorrectly()
        {
            var ind1 = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i+1).ToArray()),
                ind.Fitness + 1);
            
            popConstLenDvInitInd.AddIndividual(ind1);
            
            Assert.Equal(ind, popConstLenDvInitInd.Best());
            Assert.Equal(ind1, popConstLenDvInitInd.Worst());
        }
        
        [Fact]
        public void ConstantLengthDvPop_AddBetterIndividual_SortsCorrectly()
        {
            var ind1 = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i+1).ToArray()),
                ind.Fitness - 1);
            
            popConstLenDvInitInd.AddIndividual(ind1);
            
            Assert.Equal(ind1, popConstLenDvInitInd.Best());
            Assert.Equal(ind, popConstLenDvInitInd.Worst());
        }

        [Fact]
        public void ConstantLengthDvPop_ReplaceWorstIndividual_SortsCorrectly()
        {
            var bestInd = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i + 1).ToArray()),
                ind.Fitness - 1);

            popConstLenDvInitInd.AddIndividual(bestInd);

            var evenBetterInd = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i + 1).ToArray()),
                ind.Fitness - 2);

            popConstLenDvInitInd.ReplaceWorst(evenBetterInd);

            Assert.Equal(evenBetterInd, popConstLenDvInitInd.Best());
            Assert.Equal(bestInd, popConstLenDvInitInd.Worst());
        }

        [Fact]
        public void ConstantLengthDvPop_GetFitnesses_ReturnsSortedList()
        {
            var ind1 = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i+1).ToArray()),
                ind.Fitness - 1);
            
            popConstLenDvInitInd.AddIndividual(ind1);

            var fitnesses = popConstLenDvInitInd.GetMemberFitnesses();
            Assert.True(fitnesses.Count() == 2);
            Assert.Equal(ind1.Fitness, fitnesses.ElementAt(0));
        }
        
        [Fact]
        public void ConstantLengthDvPop_GetScores_ReturnsSortedList()
        {
            var ind1 = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i+1).ToArray()),
                ind.Fitness - 1);
            
            popConstLenDvInitInd.AddIndividual(ind1);

            var scores = popConstLenDvInitInd.GetMemberScores();
            Assert.True(scores.Count() == 2);
            Assert.Equal(ind1.Score, scores.ElementAt(0));
        }
        
        [Fact]
        public void ConstantLengthDvPop_GetSolutionVectors_ReturnsSortedList()
        {
            var ind1 = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i+1).ToArray()),
                ind.Fitness - 1);
            
            popConstLenDvInitInd.AddIndividual(ind1);

            var solutionVectors = popConstLenDvInitInd.GetMemberSolutionVectors();
            Assert.True(solutionVectors.Count() == 2);
            Assert.Equal(ind1.SolutionVector, solutionVectors.ElementAt(0));
        }
        
        [Fact]
        public void ConstantLengthDvPop_GetDecisionVectors_ReturnsSortedList()
        {
            var ind1 = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(
                    ind.DecisionVector.Vector.Select(i => (double)i+1).ToArray()),
                ind.Fitness - 1);
            
            popConstLenDvInitInd.AddIndividual(ind1);

            var decisionVectors = popConstLenDvInitInd.GetMemberDecisionVectors();
            Assert.True(decisionVectors.Count() == 2);
            Assert.Equal(ind1.DecisionVector.Vector, decisionVectors.ElementAt(0));
        }
    }
}
