using Optimisation.Base.Variables;
using System.Collections.Generic;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class IndividualTests
    {
        private readonly Individual ind;

        public IndividualTests()
        {
            var ds = DecisionSpace.CreateForUniformDoubleArray(2, 0, 2);
            var vector = new double[] { 1, 1.5 };
            var dv = new DecisionVector(ds, vector);
            ind = new Individual(dv);
        }

        [Fact]
        public void NewIndividual_StateIsNew()
        {
            Assert.Equal(IndividualStates.New, ind.State);
        }

        [Fact]
        public void EqualsOtherIndividual_HasSameDecisionVector_IsEqual()
        {

            var ds = DecisionSpace.CreateForUniformDoubleArray(2, 0, 2);
            var vector = new double[] { 1, 1.5 };
            var dv = new DecisionVector(ds, vector);
            var ind2 = new Individual(dv);

            Assert.Equal(ind2, ind);
        }

    }
}
