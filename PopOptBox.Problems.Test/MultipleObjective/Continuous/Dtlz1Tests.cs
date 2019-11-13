using System;
using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Variables;
using Xunit;

namespace PopOptBox.Problems.MultipleObjective.Continuous.Test
{
    public class Dtlz1Tests
    {
        [Fact]
        public void FewerInputDimensionsThanObjectives_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Dtlz1(2, 3));
        }
        
        [Fact]
        public void CorrectlyIdentifiesLegalSolution()
        {
            var evaluator = new Dtlz1(4,2);
            var ds = evaluator.GetOptimalParetoFront(1).ElementAt(0).GetDecisionSpace();
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds,
                ds.Select(d => d.GetNextRandom(new SystemRandomSource()))));
            Assert.True(legal);
        }

        [Fact]
        public void CorrectlyIdentifiesIllegalSolution()
        {
            var evaluator = new Dtlz1(4, 2);
            var ds = DecisionSpace.CreateForUniformDoubleArray(100, -2, -1, -2, -1);
            var legal = evaluator.GetLegality(DecisionVector.CreateFromArray(ds,
                ds.Select(d => d.GetNextRandom(new SystemRandomSource()))));
            Assert.False(legal);
        }
    }
}