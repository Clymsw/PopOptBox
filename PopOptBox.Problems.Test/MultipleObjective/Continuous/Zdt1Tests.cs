using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Problems.MultipleObjective.Continuous.Test
{
    public class Zdt1Tests
    {
        [Fact]
        public void CorrectlyIdentifiesLegalSolution()
        {
            var evaluator = new Zdt1();
            var ds = evaluator.GetOptimalParetoFront(1).ElementAt(0).GetDecisionSpace();
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds,
                ds.Select(d => d.GetNextRandom(new SystemRandomSource()))));
            Assert.True(legal);
        }

        [Fact]
        public void CorrectlyIdentifiesIllegalSolution()
        {
            var evaluator = new Zdt1();
            var ds = DecisionSpace.CreateForUniformDoubleArray(30, -2, -1, -2, -1);
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds,
                ds.Select(d => d.GetNextRandom(new SystemRandomSource()))));
            Assert.False(legal);
        }
        
        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0, 1.0 })]
        [InlineData(new[] { 0.25, 0.0, 0.25, 0.5 })]
        [InlineData(new[] { 0.36, 0.0, 0.36, 0.4 })]
        [InlineData(new[] { 0.81, 0.0, 0.81, 0.1 })]
        [InlineData(new[] { 0.019, 0.1, 0.019, 1.71 })]
        public void EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Zdt1();
            
            var ds = DecisionSpace.CreateForUniformDoubleArray(30, 0, 1, 0, 1);
            var xm = Enumerable.Repeat(values[1], 29);
            var x = new List<double> {values[0]};
            x.AddRange(xm);
            
            var result = evaluator.Evaluate(DecisionVector.CreateFromArray(ds, x));
            
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 1e-6);
            Assert.True(Math.Abs(values[3] - result.ElementAt(1)) < 1e-6);
        }

        [Theory]
        [InlineData(new double[] {0.0000360993,0.9939917271, 0.1202737561,0.6531949307, 0.3566538692,0.4027949521, 0.6557341303,0.1902258770, 1.0000000000,0.0000000000})]
        public void GeneratesOptimalFrontCorrectly(double[] values)
        {
            var evaluator = new Zdt1(2);

            var numPoints = values.Length / 2;

            var front = evaluator.GetOptimalParetoFront(numPoints);
            var frontSols = front.Select(x => evaluator.Evaluate(x));

            int p = 0;
            foreach (var point in frontSols)
            {
                var testPoint1 = values[p * 2];
                var testPoint2 = values[1 + p * 2];
                Assert.True(Math.Abs(testPoint1 - point.ElementAt(0)) < 1e-3);
                Assert.True(Math.Abs(testPoint2 - point.ElementAt(1)) < 1e-3);
                p++;
            }

        }
    }
}