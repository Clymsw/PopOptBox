using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Problems.MultipleObjective.Continuous.Test
{
    public class Zdt2Tests
    {
        [Fact]
        public void CorrectlyIdentifiesLegalSolution()
        {
            var evaluator = new Zdt2();
            var ds = evaluator.GetOptimalParetoFront(1).ElementAt(0).GetDecisionSpace();
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds,
                ds.Select(d => d.GetNextRandom(new SystemRandomSource()))));
            Assert.True(legal);
        }

        [Fact]
        public void CorrectlyIdentifiesIllegalSolution()
        {
            var evaluator = new Zdt2();
            var ds = DecisionSpace.CreateForUniformDoubleArray(30, -2, -1, -2, -1);
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds,
                ds.Select(d => d.GetNextRandom(new SystemRandomSource()))));
            Assert.False(legal);
        }
        
        [Theory]
        [InlineData(new[] { 0.0, 0.0, 0.0, 1.0 })]
        [InlineData(new[] { 0.2, 0.0, 0.2, 0.96 })]
        [InlineData(new[] { 0.5, 0.0, 0.5, 0.75 })]
        [InlineData(new[] { 0.9, 0.0, 0.9, 0.19 })]
        [InlineData(new[] { 0.19, 0.1, 0.19, 1.881 })]
        public void EvaluatesCorrectValues(double[] values)
        {
            var evaluator = new Zdt2();
            
            var ds = DecisionSpace.CreateForUniformDoubleArray(30, 0, 1, 0, 1);
            var xm = Enumerable.Repeat(values[1], 29);
            var x = new List<double> {values[0]};
            x.AddRange(xm);
            
            var result = evaluator.Evaluate(DecisionVector.CreateFromArray(ds, x));
            
            Assert.True(Math.Abs(values[2] - result.ElementAt(0)) < 1e-6);
            Assert.True(Math.Abs(values[3] - result.ElementAt(1)) < 1e-6);
        }
    }
}