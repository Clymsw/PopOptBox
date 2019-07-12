using System;
using System.Linq;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class OptimiserTests
    {
        private readonly Optimiser optimiserMock;
        private readonly ObjectCreators.OptimiserBuilderMock builder;
        
        public OptimiserTests()
        {
            builder = new ObjectCreators.OptimiserBuilderMock();
            optimiserMock = builder.CreateOptimiser();
        }

        [Fact]
        public void NewIndividualCreation_HasCorrectProperties()
        {
            var newInds = optimiserMock.GetNextToEvaluate(1);
            Assert.Equal(1, newInds.Count);

            var newInd = newInds.ElementAt(0);
            Assert.Equal(builder.DecVec, 
                newInd.DecisionVector.Vector.Select(v => (double)v));
            Assert.Equal(IndividualStates.New, newInd.State);

            var creationTime = newInd.GetProperty<DateTime>(OptimiserPropertyNames.CreationTime);
            Assert.True(creationTime < DateTime.Now);
        }

        [Fact]
        public void Reinsertion_NewIndividual_NotAllowed()
        {
            var newInd = ObjectCreators.GetIndividual(builder.DecVec);
            
            Assert.Throws<ArgumentException>(() => optimiserMock.ReInsert(new[] {newInd}));
        }
        
        [Fact]
        public void Reinsertion_EvaluatingIndividual_NotAllowed()
        {
            var newInd = ObjectCreators.GetIndividual(builder.DecVec);
            newInd.SendForEvaluation();

            Assert.Throws<ArgumentException>(() => optimiserMock.ReInsert(new[] {newInd}));
        }
        
        [Fact]
        public void Reinsertion_EvaluatedIndividual_AddedToPopulation()
        {
            var newInd = ObjectCreators.GetIndividual(builder.DecVec);
            ObjectCreators.EvaluateIndividual(newInd);
            
            Assert.Empty(optimiserMock.Population);
            
            optimiserMock.ReInsert(new[] {newInd});
            
            Assert.Collection(optimiserMock.Population, 
                i => Assert.Equal(
                    builder.DecVec, i.DecisionVector.Vector.Select(d => (double)d)));
            
            var reinsertionTime = newInd.GetProperty<DateTime>(OptimiserPropertyNames.ReinsertionTime);
            Assert.True(reinsertionTime < DateTime.Now);
        }
    }
}