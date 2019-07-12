using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Simplices.Test
{
    public class SimplexOperatorTests
    {
        private readonly OperatorMock operatorMock;

        public SimplexOperatorTests()
        {
            operatorMock = new OperatorMock(1);
        }

        [Theory]
        [InlineData(new[] { 0.5, 0.5 }, new[] { 0.0, 1 }, new[] { 1.0, 0 }, new[] { 0.0, 0 })]
        public void GetMean_TwoDim_ReturnsCorrectValue(double[] expectedAnswer, params double[][] testValues)
        {
            var ds = DecisionSpace.CreateForUniformDoubleArray(2, double.MinValue, double.MaxValue);

            var dvs = testValues.Select(v => DecisionVector.CreateFromArray(ds, v)).ToList();
            
            // Operate() is set up to call GetMean() function.
            var meanLocation = operatorMock.Operate(dvs);

            Assert.True(meanLocation.Vector.Count == expectedAnswer.Length);
            Assert.Equal(expectedAnswer, meanLocation.Vector.Select(d => (double)d).ToArray());
        }

        public class OperatorMock : SimplexOperator
        {
            public OperatorMock(double coefficient) : base(coefficient)
            {
            }

            public override DecisionVector Operate(IEnumerable<DecisionVector> orderedVertices)
            {
                return GetMean(orderedVertices);
            }

            protected override bool CheckCoefficientAcceptable(double value)
            {
                return true;
            }
        }
    }
}
