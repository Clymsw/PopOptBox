using System;
using System.Linq;
using Optimisation.Base.Variables;
using Xunit;

namespace Optimisation.Optimisers.NelderMead.Simplices.Test
{
    public class SimplexTests
    {
        [Theory]
        [InlineData(2, 1)]
        [InlineData(4, 2.1)]
        public void CreateInitialVertices_CreatesCorrectly(int numDims, double stepSize)
        {
            var initialVertex = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, double.MinValue, double.MaxValue),
                Enumerable.Repeat(0.0, numDims));
            var newVertices = Simplex.CreateInitialVertices(initialVertex, stepSize);
            
            // Collection must be D+1 long
            Assert.True(newVertices.Count == numDims + 1);
            
            // Every vector must be D long
            Assert.All(newVertices, v => Assert.True(v.Vector.Count == numDims));
            
            // The Euclidean distance of every vector from the initial one must be equal to stepSize
            Assert.All(newVertices.Skip(1),
                v => Assert.True(Math.Sqrt(
                                     v.Vector
                                         .Select((a,i) => (double)a - (double)initialVertex.Vector.ElementAt(i))
                                         .Select(a => Math.Pow(a,2))
                                         .Sum())
                                 == stepSize));
        }
    }
}