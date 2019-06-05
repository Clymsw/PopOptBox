using System;
using System.Linq;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Variables.Test
{
    public class DecisionVectorTests
    {
        private readonly DecisionSpace space;
        private const int Dims = 4;
        private const double MinValueContinuous = -3.5;
        private const double MaxValueContinuous = 2.9;
        private readonly double[] exampleContinuousVector;
        private readonly int[] exampleDiscreteVector;

        public DecisionVectorTests()
        {
            space = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinValueContinuous, MaxValueContinuous);
            
            var range = MaxValueContinuous - MinValueContinuous - 1.01;
            exampleContinuousVector = Enumerable.Range(0, Dims)
                .Select(i => MinValueContinuous + range / (Dims - 1) * i + 1)
                .ToArray();

            exampleDiscreteVector = Enumerable.Range(0, Dims)
                .Select(i => (int)Math.Ceiling(MinValueContinuous) + i)
                .ToArray();
        }

        [Fact]
        public void CreatedWithArray_ConstructsOk()
        {
            var dv = DecisionVector.CreateFromArray(space, exampleContinuousVector);

            for (var i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), exampleContinuousVector[i]);
                Assert.True(space.Dimensions.ElementAt(i).IsInBounds(dv.Vector.ElementAt(i)));
            }
        }
        
        [Fact]
        public void CreatedWithArrayOfInts_ConstructsOk()
        {
            var dv = DecisionVector.CreateFromArray(space, exampleDiscreteVector);

            for (var i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), exampleDiscreteVector[i]);
                Assert.True(space.Dimensions.ElementAt(i).IsInBounds(dv.Vector.ElementAt(i)));
            }
        }

        [Fact]
        public void CreatedWithParams_ConstructsOk()
        {
            var dv = DecisionVector.CreateFromItems(space,
                exampleContinuousVector[0], exampleContinuousVector[1], exampleContinuousVector[2], exampleContinuousVector[3]);

            for (var i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), exampleContinuousVector[i]);
            }
        }
        
        [Fact]
        public void TwoEqualVectors_WithSameSpace_AreEqual()
        {
            var values2 = exampleContinuousVector.ToArray();
            var dv1 = DecisionVector.CreateFromArray(space, exampleContinuousVector);
            var dv2 = DecisionVector.CreateFromArray(space, values2);

            Assert.Equal(dv1, dv2);
        }
        
        [Fact]
        public void TwoEqualVectors_WithDifferentSpace_AreNotEqual()
        {
            var space2 = ObjectCreators.GetDecisionSpace(Dims, MinValueContinuous - 1, MaxValueContinuous + 1);
            var dv1 = DecisionVector.CreateFromArray(space, exampleContinuousVector);
            var dv2 = DecisionVector.CreateFromArray(space2, exampleContinuousVector);

            Assert.NotEqual(dv1, dv2);
        }
        
        [Fact]
        public void TwoDifferentVectors_WithSameSpace_AreNotEqual()
        {
            var values2 = exampleContinuousVector.Select(i => i - 1).ToArray();
            var dv1 = DecisionVector.CreateFromArray(space, exampleContinuousVector);
            var dv2 = DecisionVector.CreateFromArray(space, values2);

            Assert.NotEqual(dv1, dv2);
        }
        
        [Fact]
        public void DifferenceOfTwoDifferentVectors_WithDifferentSpace_Fails()
        {
            var dv1 = DecisionVector.CreateFromArray(space, exampleContinuousVector);
            var dv2 = DecisionVector.CreateFromArray(
                ObjectCreators.GetDecisionSpace(4,MinValueContinuous - 1, MaxValueContinuous + 1),
                exampleContinuousVector);

            Assert.Throws<ArgumentException>(
                () => dv1 - dv2);
        }
        
        [Fact]
        public void DifferenceOfTwoDifferentVectors_WithSameSpace_IsCorrect()
        {
            var values2 = exampleContinuousVector.Select(i => i - 1).ToArray();
            var dv1 = DecisionVector.CreateFromArray(space, exampleContinuousVector);
            var dv2 = DecisionVector.CreateFromArray(space, values2);

            var difference = dv1 - dv2;

            for (var i = 0; i < Dims; i++)
                Assert.Equal(1, difference.ElementAt(i));
        }

        [Fact]
        public void SumOfTwoDifferentVectors_WithSameSpace_IsCorrect()
        {
            var values2 = exampleContinuousVector.Select(i => i - 1).ToArray();
            var dv1 = DecisionVector.CreateFromArray(space, exampleContinuousVector);
            var dv2 = DecisionVector.CreateFromArray(space, values2);

            var sum = dv1 + dv2;

            for (var i = 0; i < Dims; i++)
                Assert.Equal(exampleContinuousVector.ElementAt(i) * 2 - 1, sum.ElementAt(i));
        }
    }
}
