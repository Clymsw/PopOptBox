using System;
using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Optimisers.StructuredSearch.Test;
using Xunit;

namespace PopOptBox.Optimisers.StructuredSearch.Simplices.Test
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

            var inds = Helpers.CreateEvaluatedIndividualsFromArray(testValues);
            var simplex = new Simplex(
                SolutionToFitness.SingleObjectiveMinimise, Penalty.DeathPenalty,
                inds);

            var newLocation = theOperator.Operate(simplex);

            Assert.Equal(expectedAnswer, newLocation.Select(d => (double)d));
        }
    }
}