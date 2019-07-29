using Xunit;

namespace Optimisation.Base.Variables.Test
{
    public class VariableDiscreteTests
    {
        [Fact]
        public void BoundsInverted_ThrowsError()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new VariableDiscrete(2, 1));
        }

        [Fact]
        public void BoundsTheSame_ThrowsError()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new VariableDiscrete(3, 3));
        }

        [Fact]
        public void BoundsCheck_WithInvalidType_ThrowsError()
        {
            var vbl = new VariableDiscrete(1, 3);
            Assert.Throws<System.FormatException>(
                () => vbl.IsInBounds("a"));
        }

        [Fact]
        public void BoundsOk_CreatesOk()
        {
            var min = -5;
            var max = 3;
            var vbl = new VariableDiscrete(min, max);

            var range = max - min;
            Assert.True(vbl.IsInBounds(min + 1));
            Assert.True(vbl.IsInBounds(max - 1));
            Assert.False(vbl.IsInBounds(max + range));
            Assert.False(vbl.IsInBounds(min - range));
        }

        [Fact]
        public void TwoVariablesSame_AreEqual()
        {
            var min = -3;
            var max = 4;

            var vbl1 = new VariableDiscrete(min, max);
            var vbl2 = new VariableDiscrete(min, max);

            Assert.Equal(vbl1, vbl2);
        }

        [Theory]
        [InlineData(1, 3, 4)]
        [InlineData(2, 4, 1)]
        [InlineData(4, 9, 3)]
        [InlineData(4, -2, 2)]
        [InlineData(2, -4, 3)]
        public void AddOrWrap_PositiveBounds_CorrectValuesReturned(int val1, int val2, int expectedResult)
        {
            var vbl = new VariableDiscrete(1, 5);
            Assert.Equal(expectedResult, vbl.AddOrWrap(val1, val2));
        }
        
        [Theory]
        [InlineData(-3, 1, -2)]
        [InlineData(-2, 4, -3)]
        [InlineData(-1, 9, -2)]
        [InlineData(-3, -2, -5)]
        [InlineData(-5, -4, -4)]
        [InlineData(-3, -10, -3)]
        public void AddOrWrap_NegativeBounds_CorrectValuesReturned(int val1, int val2, int expectedResult)
        {
            var vbl = new VariableDiscrete(-5, -1);
            Assert.Equal(expectedResult, vbl.AddOrWrap(val1, val2));
        }
        
        [Theory]
        [InlineData(0, 3, 3)]
        [InlineData(3, 4, 0)]
        [InlineData(2, 10, -2)]
        [InlineData(1, -3, -2)]
        [InlineData(-1, -4, 2)]
        [InlineData(-1, -12, 1)]
        public void AddOrWrap_MixedBounds_CorrectValuesReturned(int val1, int val2, int expectedResult)
        {
            var vbl = new VariableDiscrete(-2, 4);
            Assert.Equal(expectedResult, vbl.AddOrWrap(val1, val2));
        }
    }
}
