using System.Linq;
using Optimisation.Base.Management;
using Moq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Base.Conversion.Test
{
    public class ModelTests
    {
        private readonly IModel modelMock;
        private const string Definition_Key = "TestDefinition";
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
            
            modelMock = new ModelMock(dv, converterMock.Object, Definition_Key);
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
                ind.GetProperty<double>(Definition_Key));
        }
    }

    internal class ModelMock : Model<double>
    {
        private readonly DecisionVector decisionVector;
        
        public ModelMock(DecisionVector decisionVector, IConverter<double> converter, string definitionKey) : base(converter, definitionKey)
        {
            this.decisionVector = decisionVector;
        }

        protected override Individual CreateNewIndividual()
        {
            return new Individual(decisionVector);
        }
    }
}