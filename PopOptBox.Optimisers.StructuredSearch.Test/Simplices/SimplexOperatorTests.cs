using System.Linq;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.StructuredSearch.Test;
using Xunit;

namespace PopOptBox.Optimisers.StructuredSearch.Simplices.Test
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
            var inds = Helpers.CreateFitnessAssessedIndividualsFromArray(testValues);
            var simplex = new Simplex(inds);
            
            // Operate() is set up to call GetMean() function.
            var meanLocation = operatorMock.Operate(simplex);

            Assert.True(meanLocation.Count == expectedAnswer.Length);
            Assert.Equal(expectedAnswer, meanLocation.Select(d => (double)d).ToArray());
        }

        public class OperatorMock : SimplexOperator
        {
            public OperatorMock(double coefficient) : base(coefficient)
            {
            }

            public override DecisionVector Operate(Simplex simplex)
            {
                return GetMean(simplex);
            }

            protected override bool CheckCoefficientAcceptable(double value)
            {
                return true;
            }
        }
    }
}
