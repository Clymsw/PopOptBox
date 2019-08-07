using System;
using Xunit;

namespace PopOptBox.Optimisers.NelderMead.Test
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
            if (errorExpected)
                Assert.Throws<ArgumentOutOfRangeException>(() => 
                    new NelderMeadSimplexOperationsManager(r,e,c,s));
            else
                new NelderMeadSimplexOperationsManager(r,e,c,s);
        } 
    }
}