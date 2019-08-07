using System;
using Xunit;

namespace PopOptBox.Optimisers.StructuredSearch.Test
{
    public class NelderMeadSimplexOperationsManagerTests
    {
        [Theory]
        [InlineData(1, 2, 0.5, 0.5, false)]
        [InlineData(0.1, 1.1, 0.1, 0.1, false)]
        [InlineData(-0.1, 1.1, 0.1, 0.1, true)]
        [InlineData(0.1, 0.9, 0.1, 0.1, true)]
        [InlineData(0.1, 1.1, -0.1, 0.1, true)]
        [InlineData(0.1, 1.1, 1.1, 0.1, true)]
        [InlineData(0.1, 1.1, 0, 0.1, true)]
        [InlineData(0.1, 1.1, 1, 0.1, true)]
        [InlineData(0.1, 1.1, 0.1, -0.1, true)]
        [InlineData(0.1, 1.1, 0.1, 1.1, true)]
        [InlineData(0.1, 1.1, 0.1, 0, true)]
        [InlineData(0.1, 1.1, 0.1, 1, true)]
        public void InvalidCoefficients_ThrowError(double r, double e, double c, double s, bool errorExpected)
        {
            var hyps = NelderMeadHyperParameters.GetDefaultHyperParameters();

            var numUpdates = 0;
            numUpdates += 
                hyps.UpdateHyperParameterValue(NelderMeadHyperParameters.Reflection_Coefficient, r)
                    ? 1 : 0;
            numUpdates += 
                hyps.UpdateHyperParameterValue(NelderMeadHyperParameters.Expansion_Coefficient, e)
                    ? 1 : 0;
            numUpdates += 
                hyps.UpdateHyperParameterValue(NelderMeadHyperParameters.Contraction_Coefficient, c)
                    ? 1 : 0;
            numUpdates += 
                hyps.UpdateHyperParameterValue(NelderMeadHyperParameters.Shrinkage_Coefficient, s)
                    ? 1 : 0;

            if (errorExpected)
            {
                if (numUpdates == 4)
                    Assert.Throws<ArgumentOutOfRangeException>(() => 
                        new NelderMeadSimplexOperationsManager(hyps));
            }
            else
            {
                Assert.Equal(4, numUpdates);
                new NelderMeadSimplexOperationsManager(hyps);
            }
        } 
    }
}