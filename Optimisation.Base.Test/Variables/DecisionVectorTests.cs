using System.Linq;
using Xunit;

namespace Optimisation.Base.Variables.Test
{
    public class DecisionVectorTests
    {
        private readonly DecisionSpace space;

        public DecisionVectorTests()
        {
            int dims = 4;
            var min = -3.5;
            var max = 2.9;

            space = DecisionSpace.CreateForUniformDoubleArray(dims, min, max);
        }

        [Fact]
        public void CreatedWithArray_ContructsOk()
        {
            var values = new[] { -2.1, 2.5, 0.4, -1 };
            var dv = DecisionVector.CreateFromArray(space, values);

            for (int i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), values[i]);
            }
        }

        [Fact]
        public void CreatedWithParams_ContructsOk()
        {
            var values = new[] { -2.1, 2.5, 0.4, -1 };
            var dv = DecisionVector.CreateFromItems(space, values[0], values[1], values[2], values[3]);

            for (int i = 0; i < space.Dimensions.Count; i++)
            {
                Assert.Equal(dv.Vector.ElementAt(i), values[i]);
            }
        }
    }
}
