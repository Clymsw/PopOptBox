using Xunit;

namespace Optimisation.Base.Variables.Test
{
    public class VariableContinuousTests
    {
        [Fact]
        public void BoundsInverted_ThrowsError()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new VariableContinuous(2, 1));
        }

        [Fact]
        public void BoundsTheSame_ThrowsError()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => new VariableContinuous(3.2, 3.2));
        }

        [Fact]
        public void BoundsCheck_WithInvalidType_ThrowsError()
        {
            var vbl = new VariableContinuous(-1.2, 4.2);
            Assert.Throws<System.FormatException>(
                () => vbl.IsInBounds("bf"));
        }

        [Fact]
        public void BoundsOk_CreatesOk()
        {
            var min = -1.4;
            var max = 2.5;
            var vbl = new VariableContinuous(min, max);

            var range = max - min;

            Assert.True(vbl.IsInBounds(min + range / 2));
            Assert.False(vbl.IsInBounds(max + range / 2));
            Assert.False(vbl.IsInBounds(min - range / 2));
        }

        [Fact]
        public void BoundsCheckNonsense_ThrowsError()
        {
            var min = -1.4;
            var max = 2.5;
            var vbl = new VariableContinuous(min, max);

            Assert.Throws<System.FormatException>(
                () => vbl.IsInBounds("nonsense"));
        }

        [Fact]
        public void TwoVariablesSame_AreEqual()
        {
            var min = -1.4;
            var max = 2.5;

            var vbl1 = new VariableContinuous(min, max);
            var vbl2 = new VariableContinuous(min, max);

            Assert.Equal(vbl1, vbl2);
        }
    }
}
