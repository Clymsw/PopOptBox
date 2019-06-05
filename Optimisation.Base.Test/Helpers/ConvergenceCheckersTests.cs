using Optimisation.Base.Management;
using Optimisation.Base.Test.Helpers;
using System.Linq;
using Xunit;

namespace Optimisation.Base.Helpers.Test
{
    public class ConvergenceCheckersTests
    {
        private readonly Population pop;
        private readonly double[] bestDv;
        private const double bestFitness = 1.5;
        private readonly double[] worstDv;
        private const double fitnessDifference = 2;
        private const double dvDifference = 0.3;

        public ConvergenceCheckersTests()
        {
            bestDv = new[] { 0.1, 0.5, 1.2 };
            worstDv = new[] { 0.1, bestDv[1] + dvDifference, 1.2 };

            var ind1 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(worstDv), bestFitness + fitnessDifference);
            var ind2 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(bestDv), bestFitness);

            pop = new Population(140, new[] { ind1, ind2 }, true);
        }

        #region AbsoluteFitnessConvergence
        [Fact]
        public void AbsoluteFitnessConvergence_MeetsCriterion_IsTrue()
        {
            Assert.True(ConvergenceCheckers.AbsoluteFitnessConvergence(pop, fitnessDifference));
        }

        [Fact]
        public void AbsoluteFitnessConvergence_FailsCriterion_IsFalse()
        {
            Assert.False(ConvergenceCheckers.AbsoluteFitnessConvergence(pop, fitnessDifference / 2));
        }
        #endregion

        #region RelativeFitnessConvergence
        [Fact]
        public void RelativeFitnessConvergence_MeetsCriterion_IsTrue()
        {
            Assert.True(ConvergenceCheckers.RelativeFitnessConvergence(pop, fitnessDifference / bestFitness));
        }

        [Fact]
        public void RelativeFitnessConvergence_FailsCriterion_IsFalse()
        {
            Assert.False(ConvergenceCheckers.RelativeFitnessConvergence(pop, fitnessDifference / bestFitness / 2));
        }
        #endregion

        #region AbsoluteDecisionVectorConvergence
        [Fact]
        public void AbsoluteDecisionVectorConvergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(ConvergenceCheckers.AbsoluteDecisionVectorConvergence(pop, dvDifference + 1E-9));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(ConvergenceCheckers.AbsoluteDecisionVectorConvergence(pop, dvDifference / 2));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_MeetsMultipleCriterion_IsTrue()
        {
            var criteria = bestDv.Select(v => dvDifference + 1E-9).ToArray();
            Assert.True(ConvergenceCheckers.AbsoluteDecisionVectorConvergence(pop, criteria));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => dvDifference/2).ToArray();
            Assert.False(ConvergenceCheckers.AbsoluteDecisionVectorConvergence(pop, criteria));
        }
        #endregion

        #region RelativeDecisionVectorDivergence
        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(ConvergenceCheckers.RelativeDecisionVectorDivergence(pop, (dvDifference + 1E-9) / bestDv[1]));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(ConvergenceCheckers.RelativeDecisionVectorDivergence(pop, dvDifference / bestDv[1] / 2));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsMultipleCriteria_IsTrue()
        {
            var criteria = bestDv.Select(v => (dvDifference + 1E-9) / v).ToArray();
            Assert.True(ConvergenceCheckers.RelativeDecisionVectorDivergence(pop, criteria));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => (dvDifference + 1E-9) / v / 2).ToArray();
            Assert.False(ConvergenceCheckers.RelativeDecisionVectorDivergence(pop, criteria));
        }
        #endregion
    }
}