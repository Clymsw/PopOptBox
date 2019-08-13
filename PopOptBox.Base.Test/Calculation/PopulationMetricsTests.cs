using System;
using System.Linq;
using PopOptBox.Base.Calculation;
using PopOptBox.Base.Helpers;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Calculation.Test
{
    public class PopulationMetricsTests
    {
        private readonly Population pop;
        private readonly double[] bestDv;
        private const double BestFitness = 1.5;
        private readonly double[] worstDv;
        private const double FitnessDifference = 2;
        private const double DvDifference = 0.3;

        public PopulationMetricsTests()
        {
            bestDv = new[] { 0.1, 0.5, 1.2 };
            worstDv = new[] { 0.1, bestDv[1] + DvDifference, 1.2 };

            var ind1 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(worstDv), BestFitness + FitnessDifference);
            ind1.SetFitness(SolutionToFitnessSingleObjective.Minimise);
            var ind2 = ObjectCreators.EvaluateIndividual(ObjectCreators.GetIndividual(bestDv), BestFitness);
            ind2.SetFitness(SolutionToFitnessSingleObjective.Minimise);

            pop = ObjectCreators.GetEmptyPopulation(140, true);
            pop.AddIndividual(ind1);
            pop.AddIndividual(ind2);
        }

        #region Centroid

        [Fact]
        public void Centroid_DifferentLengthDecisionVectors_Throws()
        {
            var newInd = ObjectCreators.EvaluateIndividual(
                ObjectCreators.GetIndividual(Enumerable.Repeat<double>(1.2, bestDv.Length+1)), 
                BestFitness + FitnessDifference);
            newInd.SetFitness(SolutionToFitnessSingleObjective.Minimise);
            
            var pop2 = ObjectCreators.GetEmptyPopulation(5, false);
            pop2.AddIndividual(pop.Best());
            pop2.AddIndividual(newInd);
            Assert.Throws<InvalidOperationException>(() => pop2.Centroid());
        }

        [Fact]
        public void Centroid_CalculatesCorrectly()
        {
            Assert.Equal(bestDv.Select((v,i) => 0.5 * (v + worstDv[i])), pop.Centroid());
        }
        
        #endregion
            
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