using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Base.Management.Test
{
    public class HyperParameterManagerTests
    {
        private readonly HyperParameterManager hyperParameters;
        private readonly List<IVariable> hyperParams;
        private readonly List<double> values;
        private const string Prefix = "param ";

        public HyperParameterManagerTests()
        {
            hyperParams = new List<IVariable>();
            values = new List<double>();

            for (var i = 0; i < 3; i++)
            {
                hyperParams.Add(new VariableContinuous(name: Prefix + $"{i}"));
                values.Add(i);
            }
            
            hyperParameters = new HyperParameterManager();
        }

        [Fact]
        public void NewHyperParameterManager_HasNoHyperParameters()
        {
            Assert.True(hyperParameters.GetHyperParameterNames().Length == 0);
        }
        
        [Fact]
        public void AddOneHyperParameter_SuccessfullySaved()
        {
            hyperParameters.AddOrReplaceHyperParameter(hyperParams.ElementAt(0), values.ElementAt(0));
            
            Assert.True(hyperParameters.GetHyperParameterNames().Length == 1);
            
            Assert.Equal(values.ElementAt(0),
                hyperParameters.GetHyperParameterValue<double>(Prefix + "0"));
        }
        
        [Fact]
        public void AddSeveralHyperParameters_SuccessfullySaved()
        {
            for (var i = 0; i < hyperParams.Count; i++)
            {
                hyperParameters.AddOrReplaceHyperParameter(hyperParams.ElementAt(i), values.ElementAt(i));
            }

            Assert.Equal(hyperParams.Count, hyperParameters.GetHyperParameterNames().Length);

            for (var i = 0; i < hyperParams.Count; i++)
            {
                Assert.Equal(values.ElementAt(i), 
                    hyperParameters.GetHyperParameterValue<double>(Prefix + $"{i}"));
            }
            
        }
        
        [Fact]
        public void ReplaceAHyperParameter_SuccessfullySaved()
        {
            // Set
            for (var i = 0; i < hyperParams.Count; i++)
            {
                hyperParameters.AddOrReplaceHyperParameter(
                    hyperParams.ElementAt(i), values.ElementAt(i));
            }

            // Replace
            var newValue = values.ElementAt(0) + 1.2;
            
            Assert.True(hyperParameters.AddOrReplaceHyperParameter(
                hyperParams.ElementAt(0), newValue));

            // Test
            Assert.Equal(hyperParams.Count, hyperParameters.GetHyperParameterNames().Length);

            Assert.Equal(newValue, 
                hyperParameters.GetHyperParameterValue<double>(Prefix + "0"));
        }
        
        [Fact]
        public void UpdateAHyperParameter_SuccessfullySaved()
        {
            // Set
            for (var i = 0; i < hyperParams.Count; i++)
            {
                hyperParameters.AddOrReplaceHyperParameter(
                    hyperParams.ElementAt(i), values.ElementAt(i));
            }

            // Update
            var newValue = values.ElementAt(0) + 1.2;
            
            Assert.True(hyperParameters.UpdateHyperParameterValue(
                Prefix + "0", newValue));

            // Test
            Assert.Equal(hyperParams.Count, hyperParameters.GetHyperParameterNames().Length);

            Assert.Equal(newValue, 
                hyperParameters.GetHyperParameterValue<double>(Prefix + "0"));
        }
    }
}