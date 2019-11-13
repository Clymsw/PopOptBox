using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination.Test
{
    public class RecombinationParentCentricTests
    {
        private readonly List<DecisionVector> parents;

        public RecombinationParentCentricTests()
        {
            var decisionSpaceUniform = DecisionSpace.CreateForUniformDoubleArray(
                2, 0.0, 5.0);

            parents = new List<DecisionVector>
            {
                DecisionVector.CreateFromArray(
                    decisionSpaceUniform, new[] {2.0, 2.0}),
                DecisionVector.CreateFromArray(
                    decisionSpaceUniform, new[] {4.0, 3.0}),
                DecisionVector.CreateFromArray(
                    decisionSpaceUniform, new[] {3.0, 4.0})
            };
        }
        
        [Fact]
        public void Operate_SmallSpreadingCoefficients_ReturnsNearMother()
        {
            var pcx = new RecombinationParentCentric(0.2, 0.2);
            var children = new List<DecisionVector>();
            for (var i = 0; i < 100; i++)
            {
                children.Add(pcx.Operate(parents.ToArray()));
            }

            var distances = children.Select(c => Distance.Euclidean(
                c.Select(d => (double)d).ToArray(), 
                parents[0].Select(d => (double)d).ToArray()));
            Assert.True(distances.All(d => d < 1e-2));
        }
    }
}