using System;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Simplices.Test
{
    public class ShrinkTests
    {
        [Theory]
        [InlineData(-20, true)]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(0.5, false)]
        public void InvalidCoefficients_ThrowError(double coefficient, bool errorExpected)
        {
            if (errorExpected)
                Assert.Throws<ArgumentOutOfRangeException>(() => new Shrink(coefficient));
            else
                new Shrink(coefficient);
        }
        
    }
}