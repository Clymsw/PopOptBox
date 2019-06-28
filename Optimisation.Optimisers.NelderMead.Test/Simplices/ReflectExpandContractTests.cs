using System;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Simplices.Test
{
    public class ReflectExpandContractTests
    {
        [Theory]
        [InlineData(-20, true)]
        [InlineData(-1, true)]
        [InlineData(0, false)]
        [InlineData(20, false)]
        public void InvalidCoefficients_ThrowError(double coefficient, bool errorExpected)
        {
            if (errorExpected)
                Assert.Throws<ArgumentOutOfRangeException>(() => new ReflectExpandContract(coefficient));
            else
                new ReflectExpandContract(coefficient);
        }
        
    }
}