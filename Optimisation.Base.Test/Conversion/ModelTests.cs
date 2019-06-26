using System.Linq;
using Optimisation.Base.Management;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Conversion.Test
{
    public class ModelTests
    {
        private readonly IModel modelMock;
        private readonly ObjectCreators.OptimiserBuilderMock builder;

        public ModelTests()
        {
            builder = new ObjectCreators.OptimiserBuilderMock();
            
            modelMock = new ObjectCreators.ModelMock(
                ObjectCreators.GetDecisionVector(builder.DecVec), 
                builder.GetConverterMock());
        }

        [Fact]
        public void NewIndividualCreation_CallsMethodCorrectly()
        {
            var ind = modelMock.GetNewIndividual();
            
            Assert.Equal(IndividualStates.New, ind.State);
            
            Assert.Equal(builder.DecVec, 
                ind.DecisionVector.Vector.Select(v => (double)v));
        }
        
        [Fact]
        public void IndividualPreparedForEvaluation_HasCorrectState()
        {
            var ind = modelMock.GetNewIndividual();
            modelMock.PrepareForEvaluation(ind);
            Assert.Equal(IndividualStates.Evaluating, ind.State);
            Assert.Equal(builder.DecVec.ElementAt(0), 
                ind.GetProperty<double>(ObjectCreators.Definition_Key));
        }
    }
}