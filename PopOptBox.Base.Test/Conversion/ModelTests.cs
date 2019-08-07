using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Conversion.Test
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
            var ind = new Individual(modelMock.GetNewDecisionVector());
            
            Assert.Equal(IndividualState.New, ind.State);
            
            Assert.Equal(builder.DecVec, 
                ind.DecisionVector.Select(v => (double)v));
        }
        
        [Fact]
        public void IndividualPreparedForEvaluation_HasCorrectState()
        {
            var ind = new Individual(modelMock.GetNewDecisionVector());
            modelMock.PrepareForEvaluation(ind);
            Assert.Equal(IndividualState.Evaluating, ind.State);
            Assert.Equal(builder.DecVec.ElementAt(0), 
                ind.GetProperty<double>(ObjectCreators.Definition_Key));
        }
    }
}