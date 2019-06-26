using System.Linq;
using Optimisation.Base.Management;
using Moq;
using Optimisation.Base.Test.Helpers;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Base.Conversion.Test
{
    public class ModelTests
    {
        private readonly IModel modelMock;
        private readonly double[] decisionVector;

        public ModelTests()
        {
            decisionVector = new[] {1.2};
            
            var dv = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(this.decisionVector.Length, double.MinValue, double.MaxValue),
                this.decisionVector);

            var converterMock = new Mock<IConverter<double>>();
            
            converterMock.Setup(x => x.ConvertToReality(dv))
                .Returns(decisionVector.ElementAt(0));
            
            modelMock = new ObjectCreators.ModelMock(dv, converterMock.Object);
        }

        [Fact]
        public void NewIndividualCreation_CallsMethodCorrectly()
        {
            var ind = modelMock.GetNewIndividual();
            
            Assert.Equal(IndividualStates.New, ind.State);
            
            Assert.Equal(decisionVector, 
                ind.DecisionVector.Vector.Select(v => (double)v));
        }
        
        [Fact]
        public void IndividualPreparedForEvaluation_HasCorrectState()
        {
            var ind = modelMock.GetNewIndividual();
            modelMock.PrepareForEvaluation(ind);
            Assert.Equal(IndividualStates.Evaluating, ind.State);
            Assert.Equal(decisionVector.ElementAt(0), 
                ind.GetProperty<double>(ObjectCreators.Definition_Key));
        }
    }
}