using System;
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
        
        [Theory]
        [InlineData(1.4, 2.4, 3.8)]
        [InlineData(2.6, 3.2, 2.4)]
        [InlineData(3.0, 6.0, 2.2)]
        [InlineData(4.2, -1.7, 2.5)]
        [InlineData(1.8, -2.6, 2.6)]
        public void AddOrWrap_PositiveBounds_CorrectValuesReturned(double val1, double val2, double expectedResult)
        {
            var vbl = new VariableContinuous(1.2, 4.6);
            var result = vbl.AddOrWrap(val1, val2);
            Assert.True(Math.Abs(expectedResult - (double)result) < 1e-6);
        }
        
        [Theory]
        [InlineData(-1.0, 0.8, -0.2)]
        [InlineData(-1.5, 2.2, -2.8)]
        [InlineData(-1.2, 8.0, -0.2)]
        [InlineData(-1.2, -1.7, -2.9)]
        [InlineData(-3.5, -3.2, -3.2)]
        public void AddOrWrap_NegativeBounds_CorrectValuesReturned(double val1, double val2, double expectedResult)
        {
            var vbl = new VariableContinuous(-3.7, -0.2);
            var result = vbl.AddOrWrap(val1, val2);
            Assert.True(Math.Abs(expectedResult - (double)result) < 1e-6);
        }
        
        [Theory]
        [InlineData(-1.0, 2.0, 1.0)]
        [InlineData(0.5, 2.6, -0.9)]
        [InlineData(-0.1, 7.2, -0.9)]
        [InlineData(0.9, -2.1, -1.2)]
        [InlineData(-1.8, -6.2, 0.0)]
        public void AddOrWrap_MixedBounds_CorrectValuesReturned(double val1, double val2, double expectedResult)
        {
            var vbl = new VariableContinuous(-2.0, 2.0);
            var result = vbl.AddOrWrap(val1, val2);
            Assert.True(Math.Abs(expectedResult - (double)result) < 1e-6);
        }
    }
}
