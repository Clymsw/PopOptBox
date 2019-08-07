using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Test.Helpers;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Base.Management.Test
{
    public class OptimiserBuilderTests
    {
        private readonly OptimiserBuilder optBuilder;
        private readonly List<IVariable> hyperParams;
        private readonly List<double> values;

        public OptimiserBuilderTests()
        {
            hyperParams = new List<IVariable>();
            values = new List<double>();

            for (var i = 0; i < 3; i++)
            {
                hyperParams.Add(new VariableContinuous(name: $"param {i}"));
                values.Add(i);
            }
            
            optBuilder = new ObjectCreators.OptimiserBuilderMock();
        }

        [Fact]
        public void NewOptimiserBuilder_HasNoHyperParameters()
        {
            Assert.Equal(0, optBuilder.HyperParameters.Vector.Count);
        }
        
        [Fact]
        public void AddOneHyperParameter_SuccessfullySaved()
        {
            optBuilder.AddHyperParameter(hyperParams.ElementAt(0), values.ElementAt(0));
            
            Assert.Equal(1, optBuilder.HyperParameters.Vector.Count);
            Assert.Equal(values.ElementAt(0), optBuilder.HyperParameters.Vector.ElementAt(0));
            Assert.Equal(hyperParams.ElementAt(0).Name, 
                optBuilder.HyperParameters.GetDecisionSpace().Dimensions.ElementAt(0).Name);
        }
        
        [Fact]
        public void AddSeveralHyperParameters_SuccessfullySaved()
        {
            for (var i = 0; i < hyperParams.Count; i++)
            {
                optBuilder.AddHyperParameter(hyperParams.ElementAt(i), values.ElementAt(i));
            }

            Assert.Equal(hyperParams.Count, optBuilder.HyperParameters.Vector.Count);

            for (var i = 0; i < hyperParams.Count; i++)
            {
                Assert.Equal(values.ElementAt(i), 
                    optBuilder.GetHyperParameterValue(hyperParams.ElementAt(i).Name));
            }
            
        }
    }
}