using Optimisation.Base.Variables;
using System;
using System.Linq;
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

        [Theory]
        [InlineData(new[] { 0, 0.5 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void Shrink_CalculatesCorrectNewLocation(double[] expectedAnswer, params double[][] testValues)
        {
            var theOperator = new Shrink(0.5);

            var ds = DecisionSpace.CreateForUniformDoubleArray(expectedAnswer.Length, double.MinValue, double.MaxValue);
            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v)).ToList();

            var newLocation = theOperator.Operate(dvs);

            Assert.Equal(expectedAnswer, newLocation.Vector.Select(d => (double)d));
        }
    }
}