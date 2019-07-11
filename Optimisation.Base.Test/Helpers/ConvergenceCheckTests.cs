using Optimisation.Base.Management;
using Optimisation.Base.Test.Helpers;
using System.Linq;
using Xunit;

namespace Optimisation.Base.Helpers.Test
{
    public class ConvergenceCheckTests
    {
        private readonly Population pop;
        private readonly double[] bestDv;
        private const double BestFitness = 1.5;
        private readonly double[] worstDv;
        private const double FitnessDifference = 2;
        private const double DvDifference = 0.3;

        public ConvergenceCheckTests()
        {
            bestDv = new[] { 0.1, 0.5, 1.2 };
            worstDv = new[] { 0.1, bestDv[1] + DvDifference, 1.2 };

            var ind1 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(worstDv), BestFitness + FitnessDifference);
            var ind2 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(bestDv), BestFitness);

            pop = new Population(140, new[] { ind1, ind2 }, true);
        }

        #region AbsoluteFitness
        [Fact]
        public void AbsoluteFitness_MeetsCriterion_IsTrue()
        {
            Assert.True(pop.AbsoluteFitness(FitnessDifference));
        }

        [Fact]
        public void AbsoluteFitness_FailsCriterion_IsFalse()
        {
            Assert.False(pop.AbsoluteFitness(FitnessDifference / 2));
        }
        #endregion

        #region RelativeFitness
        [Fact]
        public void RelativeFitness_MeetsCriterion_IsTrue()
        {
            Assert.True(pop.RelativeFitness(FitnessDifference / BestFitness));
        }

        [Fact]
        public void RelativeFitness_FailsCriterion_IsFalse()
        {
            Assert.False(pop.RelativeFitness(FitnessDifference / BestFitness / 2));
        }
        #endregion

        #region AbsoluteDecisionVector
        [Fact]
        public void AbsoluteDecisionVector_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(pop.AbsoluteDecisionVector(DvDifference + 1E-9));
        }

        [Fact]
        public void AbsoluteDecisionVector_FailsSingleCriterion_IsFalse()
        {
            Assert.False(pop.AbsoluteDecisionVector(DvDifference / 2));
        }

        [Fact]
        public void AbsoluteDecisionVector_MeetsMultipleCriterion_IsTrue()
        {
            var criteria = bestDv.Select(v => DvDifference + 1E-9).ToArray();
            Assert.True(pop.AbsoluteDecisionVector(criteria));
        }

        [Fact]
        public void AbsoluteDecisionVector_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => DvDifference/2).ToArray();
            Assert.False(pop.AbsoluteDecisionVector(criteria));
        }
        #endregion

        #region RelativeDecisionVectorDivergence
        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(pop.RelativeDecisionVector((DvDifference + 1E-9) / bestDv[1]));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(pop.RelativeDecisionVector(DvDifference / bestDv[1] / 2));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsMultipleCriteria_IsTrue()
        {
            var criteria = bestDv.Select(v => (DvDifference + 1E-9) / v).ToArray();
            Assert.True(pop.RelativeDecisionVector(criteria));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => (DvDifference + 1E-9) / v / 2).ToArray();
            Assert.False(pop.RelativeDecisionVector(criteria));
        }
        #endregion
    }
}