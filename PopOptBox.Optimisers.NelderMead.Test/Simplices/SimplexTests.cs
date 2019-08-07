using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.Optimisers.NelderMead.Test;
using Xunit;

namespace PopOptBox.Optimisers.NelderMead.Simplices.Test
{
    public class SimplexTests
    {
        [Theory]
        [InlineData(1, 0.1)]
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
            Assert.All(newVertices, v => Assert.True(v.Count == numDims));
            
            // The Euclidean distance of every vector from the initial one must be equal to stepSize
            Assert.All(newVertices.Skip(1),
                v => Assert.True(Math.Sqrt(v
                                     .Select((a,i) => (double)a - (double)initialVertex.ElementAt(i))
                                     .Select(a => Math.Pow(a,2))
                                     .Sum())
                                 == stepSize));
        }

        [Fact]
        public void CreateInitialVertices_WithStepSizeZero_Throws()
        {
            var initialVertex = DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(3, double.MinValue, double.MaxValue),
                Enumerable.Repeat(0.0, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => Simplex.CreateInitialVertices(initialVertex, 0));
        }

        [Fact]
        public void Construction_WithWrongLengthSimplex_Throws()
        {
            // Create a 2D problem with only one vertex.
            var testInds = new List<Individual>
            {
                new Individual(
                    DecisionVector.CreateFromArray(
                        DecisionSpace.CreateForUniformDoubleArray(2,double.MinValue,double.MaxValue),
                        new[]{2.0, 2.0}))
            };
            foreach (var ind in testInds)
            {
                Helpers.EvaluateIndividual(ind);
            }
            
            Assert.Throws<ArgumentException>(() => new Simplex(testInds));
        }
        
        [Fact]
        public void Construction_WithInvalidVertexTypes_Throws()
        {
            // Create a 1D problem with two discrete vertices.
            var testInds = new List<Individual>
            {
                new Individual(
                    DecisionVector.CreateFromArray(
                        DecisionSpace.CreateForUniformIntArray(1,int.MinValue,int.MaxValue),
                        new[]{1})),
                new Individual(
                    DecisionVector.CreateFromArray(
                        DecisionSpace.CreateForUniformIntArray(1,int.MinValue,int.MaxValue),
                        new[]{2}))
            };
            foreach (var ind in testInds)
            {
                Helpers.EvaluateIndividual(ind);
            }
            
            Assert.Throws<ArgumentException>(() => new Simplex(testInds));
        }
        
        [Fact]
        public void Construction_WithDifferentVertexDimensionality_Throws()
        {
            // Create a simplex with two continuous vertices, where one is 1D and the other 2D.
            var testInds = new List<Individual>
            {
                new Individual(
                    DecisionVector.CreateFromArray(
                        DecisionSpace.CreateForUniformDoubleArray(1,double.MinValue,double.MaxValue),
                        new[]{1.0})),
                new Individual(
                    DecisionVector.CreateFromArray(
                        DecisionSpace.CreateForUniformDoubleArray(2,double.MinValue,double.MaxValue),
                        new[]{1.0, 2.0}))
            };
            foreach (var ind in testInds)
            {
                Helpers.EvaluateIndividual(ind);
            }
            
            Assert.Throws<ArgumentException>(() => new Simplex(testInds));
        }
    }
}