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
        private const double BestFitness = 1.5;
        private readonly double[] worstDv;
        private const double FitnessDifference = 2;
        private const double DvDifference = 0.3;

        public ConvergenceCheckersTests()
        {
            bestDv = new[] { 0.1, 0.5, 1.2 };
            worstDv = new[] { 0.1, bestDv[1] + DvDifference, 1.2 };

            var ind1 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(worstDv), BestFitness + FitnessDifference);
            var ind2 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(bestDv), BestFitness);

            pop = new Population(140, new[] { ind1, ind2 }, true);
        }

        #region AbsoluteFitnessConvergence
        [Fact]
        public void AbsoluteFitnessConvergence_MeetsCriterion_IsTrue()
        {
            Assert.True(pop.AbsoluteFitnessConvergence(FitnessDifference));
        }

        [Fact]
        public void AbsoluteFitnessConvergence_FailsCriterion_IsFalse()
        {
            Assert.False(pop.AbsoluteFitnessConvergence(FitnessDifference / 2));
        }
        #endregion

        #region RelativeFitnessConvergence
        [Fact]
        public void RelativeFitnessConvergence_MeetsCriterion_IsTrue()
        {
            Assert.True(pop.RelativeFitnessConvergence(FitnessDifference / BestFitness));
        }

        [Fact]
        public void RelativeFitnessConvergence_FailsCriterion_IsFalse()
        {
            Assert.False(pop.RelativeFitnessConvergence(FitnessDifference / BestFitness / 2));
        }
        #endregion

        #region AbsoluteDecisionVectorConvergence
        [Fact]
        public void AbsoluteDecisionVectorConvergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(pop.AbsoluteDecisionVectorConvergence(DvDifference + 1E-9));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(pop.AbsoluteDecisionVectorConvergence(DvDifference / 2));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_MeetsMultipleCriterion_IsTrue()
        {
            var criteria = bestDv.Select(v => DvDifference + 1E-9).ToArray();
            Assert.True(pop.AbsoluteDecisionVectorConvergence(criteria));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => DvDifference/2).ToArray();
            Assert.False(pop.AbsoluteDecisionVectorConvergence(criteria));
        }
        #endregion

        #region RelativeDecisionVectorDivergence
        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(pop.RelativeDecisionVectorDivergence((DvDifference + 1E-9) / bestDv[1]));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(pop.RelativeDecisionVectorDivergence(DvDifference / bestDv[1] / 2));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsMultipleCriteria_IsTrue()
        {
            var criteria = bestDv.Select(v => (DvDifference + 1E-9) / v).ToArray();
            Assert.True(pop.RelativeDecisionVectorDivergence(criteria));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => (DvDifference + 1E-9) / v / 2).ToArray();
            Assert.False(pop.RelativeDecisionVectorDivergence(criteria));
        }
        #endregion
    }
}