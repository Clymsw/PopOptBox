using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PopOptBox.Base.Variables.Test
{
    public class DecisionSpaceTests
    {
        private const int Dims = 4;
        private const int MinValueDiscrete = 2;
        private const int MaxValueDiscrete = 4;
        private const double MinValueContinuous = -3.5;
        private const double MaxValueContinuous = 2.9;
        
        [Fact]
        public void CreatedWithMixedArray_ConstructsOk()
        {
            var vbl1 = new VariableContinuous(-2.5, 2.6);
            var vbl2 = new VariableDiscrete(-6, 1);
            var vbl3 = new VariableContinuous(2, 7.4);
            var vbl4 = new VariableDiscrete(4, 12);

            var space = new DecisionSpace(new List<IVariable> { vbl1, vbl2, vbl3, vbl4 });

            Assert.Equal(4, space.Count);
            Assert.Equal(space.ElementAt(0).IsInBounds(0), vbl1.IsInBounds(0));
            Assert.Equal(space.ElementAt(1).IsInBounds(0), vbl2.IsInBounds(0));
            Assert.Equal(space.ElementAt(2).IsInBounds(0), vbl3.IsInBounds(0));
            Assert.Equal(space.ElementAt(3).IsInBounds(0), vbl4.IsInBounds(0));
        }

        [Fact]
        public void CreatedWithUniformIntArray_ConstructsOk()
        {
            var space = DecisionSpace.CreateForUniformIntArray(Dims, MinValueDiscrete, MaxValueDiscrete);

            Assert.Equal(Dims, space.Count);
            Assert.True(space.First().IsInBounds(MinValueDiscrete));
            Assert.True(space.First().IsInBounds(MaxValueDiscrete));
            Assert.False(space.First().IsInBounds(MinValueDiscrete - 1));
            Assert.False(space.First().IsInBounds(MaxValueDiscrete + 1));
        }

        [Fact]
        public void CreatedWithUniformDoubleArray_ConstructsOk()
        {
            var space = DecisionSpace.CreateForUniformDoubleArray(Dims, MinValueContinuous, MaxValueContinuous);

            var range = MaxValueContinuous - MinValueContinuous;

            Assert.Equal(Dims, space.Count);
            Assert.True(space.First().IsInBounds(MinValueContinuous + range/2));
            Assert.False(space.First().IsInBounds(MinValueContinuous - range/2));
            Assert.False(space.First().IsInBounds(MaxValueContinuous + range/2));
        }

        [Fact]
        public void TwoUniformIntArrays_WithEqualBounds_AreEqual()
        {
            var space1 = DecisionSpace.CreateForUniformIntArray(Dims, MinValueDiscrete, MaxValueDiscrete);
            var space2 = DecisionSpace.CreateForUniformIntArray(Dims, MinValueDiscrete, MaxValueDiscrete);

            Assert.Equal(space1, space2);
        }

        [Fact]
        public void TwoUniformDoubleArrays_WithEqualBounds_AreEqual()
        {
            var space1 = DecisionSpace.CreateForUniformDoubleArray(Dims, MinValueContinuous, MaxValueContinuous);
            var space2 = DecisionSpace.CreateForUniformDoubleArray(Dims, MinValueContinuous, MaxValueContinuous); 

            Assert.Equal(space1, space2);
        }

        [Fact]
        public void UniformIntAndDoubleArrays_WithEqualBounds_AreNotEqual()
        {
            var space1 = DecisionSpace.CreateForUniformIntArray(Dims, MinValueDiscrete, MaxValueDiscrete);
            var space2 = DecisionSpace.CreateForUniformDoubleArray(Dims, MinValueDiscrete, MaxValueDiscrete);

            Assert.NotEqual(space1, space2);
        }
    }
}
