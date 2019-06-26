using System;
using System.Linq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class OptimiserTests
    {
        private readonly Optimiser optimiserMock;
        private readonly double[] decisionVector;
        
        public OptimiserTests()
        {
            decisionVector = new[] {1.2};
            
            var dv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(this.decisionVector.Length, double.MinValue, double.MaxValue),
                this.decisionVector);
            
            optimiserMock = new OptimiserMock(dv,
                new Population(),
                v => v,
                v => v.ElementAt(0),
                v => 1000);
        }

        [Fact]
        public void NewIndividualCreation_HasCorrectProperties()
        {
            var newInds = optimiserMock.GetNextToEvaluate(1);
            Assert.Equal(1, newInds.Count);

            var newInd = newInds.ElementAt(0);
            Assert.Equal(decisionVector, newInd.DecisionVector.Vector.Select(v => (double)v));
            Assert.Equal(IndividualStates.New, newInd.State);

            var creationTime = newInd.GetProperty<DateTime>(OptimiserDefinitions.CreationTime);
            Assert.True(creationTime < DateTime.Now);
        }

        [Fact]
        public void Reinsertion_NewIndividual_NotAllowed()
        {
            var dv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(this.decisionVector.Length, double.MinValue, double.MaxValue),
                decisionVector);
            
            var newInd = new Individual(dv);

            Assert.Throws<ArgumentException>(() => optimiserMock.ReInsert(new[] {newInd}));
        }
        
        [Fact]
        public void Reinsertion_EvaluatingIndividual_NotAllowed()
        {
            var dv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(this.decisionVector.Length, double.MinValue, double.MaxValue),
                decisionVector);
            
            var newInd = new Individual(dv);
            newInd.SendForEvaluation();

            Assert.Throws<ArgumentException>(() => optimiserMock.ReInsert(new[] {newInd}));
        }
        
        [Fact]
        public void Reinsertion_EvaluatedIndividual_AddedToPopulation()
        {
            var dv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(this.decisionVector.Length, double.MinValue, double.MaxValue),
                decisionVector);
            
            var newInd = new Individual(dv);
            newInd.SendForEvaluation();
            newInd.FinishEvaluating();

            Assert.Empty(optimiserMock.Population);
            
            optimiserMock.ReInsert(new[] {newInd});
            
            Assert.Collection(optimiserMock.Population, 
                i => Assert.Equal(
                    decisionVector, i.DecisionVector.Vector.Select(d => (double)d)));
            
            var reinsertionTime = newInd.GetProperty<DateTime>(OptimiserDefinitions.ReinsertionTime);
            Assert.True(reinsertionTime < DateTime.Now);
        }
    }

    internal class OptimiserMock : Optimiser
    {
        private readonly DecisionVector decisionVector;
        
        public OptimiserMock(DecisionVector decisionVector,
            Population initialPopulation, 
            Func<double[], double[]> solutionToScoreDelegate, 
            Func<double[], double> scoreToFitDelegate, 
            Func<double[], double> penaltyDelegate) : base(initialPopulation, solutionToScoreDelegate, scoreToFitDelegate, penaltyDelegate)
        {
            this.decisionVector = decisionVector;
        }

        protected override DecisionVector GetNewDecisionVector()
        {
            return decisionVector;
        }

        protected override bool CheckAcceptable(Individual ind)
        {
            return true;
        }
    }
}