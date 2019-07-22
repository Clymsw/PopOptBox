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
    }
}
