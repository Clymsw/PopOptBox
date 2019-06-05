using System.Linq;
using Xunit;

namespace Optimisation.Base.Variables.Test
{
    public class DecisionVectorTests
    {
        private readonly DecisionSpace space;
        private const int Dims = 4;
        private const double MinValueContinuous = -3.5;
        private const double MaxValueContinuous = 2.9;

        public DecisionVectorTests()
        {
            space = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinValueContinuous, MaxValueContinuous);
        }

        [Fact]
        public void CreatedWithArray_ConstructsOk()
        {
            var values = new[] { -2.1, 2.5, 0.4, -1 };
            var dv = DecisionVector.CreateFromArray(space, values);

            for (var i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), values[i]);
                Assert.True(space.Dimensions.ElementAt(i).IsInBounds(dv.Vector.ElementAt(i)));
            }
        }
        
        [Fact]
        public void CreatedWithArrayOfInts_ConstructsOk()
        {
            var values = new[] { -2, 2, 0, -1 };
            var dv = DecisionVector.CreateFromArray(space, values);

            for (var i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), values[i]);
                Assert.True(space.Dimensions.ElementAt(i).IsInBounds(dv.Vector.ElementAt(i)));
            }
        }

        [Fact]
        public void CreatedWithParams_ConstructsOk()
        {
            var values = new[] { -2.1, 2.5, 0.4, -1 };
            var dv = DecisionVector.CreateFromItems(space, values[0], values[1], values[2], values[3]);

            for (var i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), values[i]);
            }
        }
        
        [Fact]
        public void TwoEqualVectors_WithSameSpace_AreEqual()
        {
            var values1 = new[] { -2.1, 2.5, 0.4, -1 };
            var values2 = values1.ToArray();
            var dv1 = DecisionVector.CreateFromArray(space, values1);
            var dv2 = DecisionVector.CreateFromArray(space, values2);

            Assert.Equal(dv1, dv2);
        }
        
        [Fact]
        public void TwoEqualVectors_WithDifferentSpace_AreNotEqual()
        {
            var values = new[] { -2.1, 2.5, 0.4, -1 };
            var space2 = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinValueContinuous - 1, MaxValueContinuous + 1);
            var dv1 = DecisionVector.CreateFromArray(space, values);
            var dv2 = DecisionVector.CreateFromArray(space2, values);

            Assert.NotEqual(dv1, dv2);
        }
        
        [Fact]
        public void TwoDifferentVectors_WithSameSpace_AreNotEqual()
        {
            var values1 = new[] { -2.1, 2.5, 0.4, -1 };
            var values2 = values1.Select(i => i - 1).ToArray();
            var dv1 = DecisionVector.CreateFromArray(space, values1);
            var dv2 = DecisionVector.CreateFromArray(space, values2);

            Assert.NotEqual(dv1, dv2);
        }
    }
}
