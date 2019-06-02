using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Optimisation.Base.Variables.Test
{
    public class DecisionSpaceTests
    {
        [Fact]
        public void CreatedWithMixedArray_ContructsOk()
        {
            var vbl1 = new VariableContinuous(-2.5, 2.6);
            var vbl2 = new VariableDiscrete(-6, 1);
            var vbl3 = new VariableContinuous(2, 7.4);
            var vbl4 = new VariableDiscrete(4, 12);

            var space = new DecisionSpace(new List<IVariable> { vbl1, vbl2, vbl3, vbl4 });

            Assert.Equal(4, space.Dimensions.Count);
            Assert.Equal(space.Dimensions.ElementAt(0).IsInBounds(0), vbl1.IsInBounds(0));
            Assert.Equal(space.Dimensions.ElementAt(1).IsInBounds(0), vbl2.IsInBounds(0));
            Assert.Equal(space.Dimensions.ElementAt(2).IsInBounds(0), vbl3.IsInBounds(0));
            Assert.Equal(space.Dimensions.ElementAt(3).IsInBounds(0), vbl4.IsInBounds(0));
        }

        [Fact]
        public void CreatedWithUniformIntArray_ContructsOk()
        {
            int dims = 4;
            int min = 2;
            int max = 4;

            var space = DecisionSpace.CreateForUniformIntArray(dims, min, max);

            Assert.Equal(dims, space.Dimensions.Count);
            Assert.True(space.Dimensions.First().IsInBounds(min));
            Assert.True(space.Dimensions.First().IsInBounds(max));
            Assert.False(space.Dimensions.First().IsInBounds(min - 1));
            Assert.False(space.Dimensions.First().IsInBounds(max + 1));
        }

        [Fact]
        public void CreatedWithUniformDoubleArray_ContructsOk()
        {
            int dims = 4;
            var min = -3.5;
            var max = 2.9;

            var space = DecisionSpace.CreateForUniformDoubleArray(dims, min, max);

            var range = max - min;

            Assert.Equal(dims, space.Dimensions.Count);
            Assert.True(space.Dimensions.First().IsInBounds(min + range/2));
            Assert.False(space.Dimensions.First().IsInBounds(min - range/2));
            Assert.False(space.Dimensions.First().IsInBounds(max + range/2));
        }

        [Fact]
        public void TwoUniformIntArrays_WithEqualBounds_AreEqual()
        {
            int dims = 4;
            int min = 2;
            int max = 4;

            var space1 = DecisionSpace.CreateForUniformIntArray(dims, min, max);
            var space2 = DecisionSpace.CreateForUniformIntArray(dims, min, max);

            Assert.Equal(space1, space2);
        }

        [Fact]
        public void TwoUniformDoubleArrays_WithEqualBounds_AreEqual()
        {
            int dims = 4;
            var min = -3.5;
            var max = 2.9;

            var space1 = DecisionSpace.CreateForUniformDoubleArray(dims, min, max);
            var space2 = DecisionSpace.CreateForUniformDoubleArray(dims, min, max); 

            Assert.Equal(space1, space2);
        }

        [Fact]
        public void UniformIntAndDoubleArrays_WithEqualBounds_AreNotEqual()
        {
            int dims = 4;
            int min = 2;
            int max = 4;

            var space1 = DecisionSpace.CreateForUniformIntArray(dims, min, max);
            var space2 = DecisionSpace.CreateForUniformDoubleArray(dims, min, max);

            Assert.NotEqual(space1, space2);
        }
    }
}
