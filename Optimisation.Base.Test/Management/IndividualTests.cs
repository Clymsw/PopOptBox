using Optimisation.Base.Variables;
using System.Linq;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class IndividualTests
    {
        private const int Dims = 2;
        private const double MinBound = 0;
        private const double MaxBound = 2;

        private readonly double[] testVector;
        
        private readonly Individual ind;

        public IndividualTests()
        {
            var ds = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinBound, MaxBound);
            testVector = new[] { 1, 1.5 };
            var dv = DecisionVector.CreateFromArray(ds, testVector);
            ind = new Individual(dv);
        }

        [Fact]
        public void NewIndividual_StateIsNew()
        {
            Assert.Equal(IndividualStates.New, ind.State);
        }

        [Fact]
        public void OtherIndividual_HasSameDecisionVector_IsEqual()
        {
            var space2 = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinBound, MaxBound);
            var vector2 = testVector.ToArray();
            var dv2 = DecisionVector.CreateFromArray(space2, vector2);
            var ind2 = new Individual(dv2);

            Assert.Equal(ind2, ind);
        }
        
        [Fact]
        public void OtherIndividual_HasDifferentDecisionVector_IsNotEqual()
        {
            var space2 = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinBound, MaxBound);
            var vector2 = testVector.Select(i => i - 0.01).ToArray();
            var dv2 = DecisionVector.CreateFromArray(space2, vector2);
            var ind2 = new Individual(dv2);

            Assert.NotEqual(ind2, ind);
        }

    }
}
