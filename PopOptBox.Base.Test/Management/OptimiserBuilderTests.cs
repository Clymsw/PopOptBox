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
            Assert.True(optBuilder.HyperParameters.Count == 0);
        }
        
        [Fact]
        public void AddOneHyperParameter_SuccessfullySaved()
        {
            optBuilder.AddHyperParameter(hyperParams.ElementAt(0), values.ElementAt(0));
            
            Assert.True(optBuilder.HyperParameters.Count == 1);
            Assert.Equal(values.ElementAt(0), optBuilder.HyperParameters.ElementAt(0));
            Assert.Equal(hyperParams.ElementAt(0).Name, 
                optBuilder.HyperParameters.GetDecisionSpace().ElementAt(0).Name);
        }
        
        [Fact]
        public void AddSeveralHyperParameters_SuccessfullySaved()
        {
            for (var i = 0; i < hyperParams.Count; i++)
            {
                optBuilder.AddHyperParameter(hyperParams.ElementAt(i), values.ElementAt(i));
            }

            Assert.Equal(hyperParams.Count, optBuilder.HyperParameters.Count);

            for (var i = 0; i < hyperParams.Count; i++)
            {
                Assert.Equal(values.ElementAt(i), 
                    optBuilder.GetHyperParameterValue(hyperParams.ElementAt(i).Name));
            }
            
        }
    }
}