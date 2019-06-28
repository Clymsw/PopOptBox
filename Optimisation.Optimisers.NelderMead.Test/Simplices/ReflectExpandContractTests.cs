using Optimisation.Base.Variables;
using System;
using System.Linq;
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

        [Theory]
        [InlineData(new[] { 1.0, 1.0 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void Reflect_CalculatesCorrectNewLocation(double[] expectedAnswer, params double[][] testValues)
        {
            var theOperator = new ReflectExpandContract(1);

            var ds = DecisionSpace.CreateForUniformDoubleArray(expectedAnswer.Length, double.MinValue, double.MaxValue);
            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v)).ToList();

            var newLocation = theOperator.Operate(dvs);

            Assert.Equal(expectedAnswer, newLocation.Vector.Select(d => (double)d));
        }

        [Theory]
        [InlineData(new[] { 1.5, 1.5 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void Expand_CalculatesCorrectNewLocation(double[] expectedAnswer, params double[][] testValues)
        {
            var theOperator = new ReflectExpandContract(2);

            var ds = DecisionSpace.CreateForUniformDoubleArray(expectedAnswer.Length, double.MinValue, double.MaxValue);
            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v)).ToList();

            var newLocation = theOperator.Operate(dvs);

            Assert.Equal(expectedAnswer, newLocation.Vector.Select(d => (double)d));
        }

        [Theory]
        [InlineData(new[] { 0.75, 0.75 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void ContractOut_CalculatesCorrectNewLocation(double[] expectedAnswer, params double[][] testValues)
        {
            var theOperator = new ReflectExpandContract(0.5);

            var ds = DecisionSpace.CreateForUniformDoubleArray(expectedAnswer.Length, double.MinValue, double.MaxValue);
            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v)).ToList();

            var newLocation = theOperator.Operate(dvs);

            Assert.Equal(expectedAnswer, newLocation.Vector.Select(d => (double)d));
        }

        [Theory]
        [InlineData(new[] { 0.25, 0.25 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void ContractIn_CalculatesCorrectNewLocation(double[] expectedAnswer, params double[][] testValues)
        {
            var theOperator = new ReflectExpandContract(-0.5);

            var ds = DecisionSpace.CreateForUniformDoubleArray(expectedAnswer.Length, double.MinValue, double.MaxValue);
            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v)).ToList();

            var newLocation = theOperator.Operate(dvs);

            Assert.Equal(expectedAnswer, newLocation.Vector.Select(d => (double)d));
        }
    }
}