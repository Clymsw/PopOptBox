using System;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.Calculation.Test
{
    public class PopulationMetricsTests
    {
        // Single objective
        private readonly Population singleObjectivePopulation;
        private readonly double[] bestDv;
        private const double BestFitness = 1.5;
        private readonly double[] worstDv;
        private const double FitnessDifference = 2;
        private const double DvDifference = 0.3;

        public PopulationMetricsTests()
        {
            // Set up single-objective population for Convergence and Centroid tests
            bestDv = new[] { 0.1, 0.5, 1.2 };
            worstDv = new[] { 0.1, bestDv[1] + DvDifference, 1.2 };

            var ind1 = ObjectCreators.EvaluateIndividualAndSetFitness(ObjectCreators.GetIndividual(worstDv), BestFitness + FitnessDifference);
            var ind2 = ObjectCreators.EvaluateIndividualAndSetFitness(ObjectCreators.GetIndividual(bestDv), BestFitness);

            singleObjectivePopulation = ObjectCreators.GetEmptyPopulation(140, true);
            singleObjectivePopulation.AddIndividual(ind1);
            singleObjectivePopulation.AddIndividual(ind2);
        }

        #region Centroid

        [Fact]
        public void Centroid_DifferentLengthDecisionVectors_Throws()
        {
            var newInd = ObjectCreators.EvaluateIndividualAndSetFitness(
                ObjectCreators.GetIndividual(Enumerable.Repeat<double>(1.2, bestDv.Length+1)), 
                BestFitness + FitnessDifference);
            
            var pop2 = ObjectCreators.GetEmptyPopulation(5, false);
            pop2.AddIndividual(singleObjectivePopulation.Best());
            pop2.AddIndividual(newInd);
            Assert.Throws<InvalidOperationException>(() => pop2.Centroid());
        }

        [Fact]
        public void Centroid_CalculatesCorrectly()
        {
            Assert.Equal(bestDv.Select((v,i) => 0.5 * (v + worstDv[i])), singleObjectivePopulation.Centroid());
        }
        
        #endregion

        #region MultiObjective

        #endregion
            
        #region AbsoluteFitnessConvergence
        
        [Fact]
        public void AbsoluteFitnessConvergence_MeetsCriterion_IsTrue()
        {
            Assert.True(singleObjectivePopulation.AbsoluteFitnessConvergence(FitnessDifference));
        }

        [Fact]
        public void AbsoluteFitnessConvergence_FailsCriterion_IsFalse()
        {
            Assert.False(singleObjectivePopulation.AbsoluteFitnessConvergence(FitnessDifference / 2));
        }
        
        #endregion

        #region RelativeFitnessConvergence
        
        [Fact]
        public void RelativeFitnessConvergence_MeetsCriterion_IsTrue()
        {
            Assert.True(singleObjectivePopulation.RelativeFitnessConvergence(FitnessDifference / BestFitness));
        }

        [Fact]
        public void RelativeFitnessConvergence_FailsCriterion_IsFalse()
        {
            Assert.False(singleObjectivePopulation.RelativeFitnessConvergence(FitnessDifference / BestFitness / 2));
        }
        
        #endregion

        #region AbsoluteDecisionVectorConvergence
        
        [Fact]
        public void AbsoluteDecisionVectorConvergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(singleObjectivePopulation.AbsoluteDecisionVectorConvergence(DvDifference + 1E-9));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(singleObjectivePopulation.AbsoluteDecisionVectorConvergence(DvDifference / 2));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_MeetsMultipleCriterion_IsTrue()
        {
            var criteria = bestDv.Select(v => DvDifference + 1E-9).ToArray();
            Assert.True(singleObjectivePopulation.AbsoluteDecisionVectorConvergence(criteria));
        }

        [Fact]
        public void AbsoluteDecisionVectorConvergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => DvDifference/2).ToArray();
            Assert.False(singleObjectivePopulation.AbsoluteDecisionVectorConvergence(criteria));
        }
        
        #endregion

        #region RelativeDecisionVectorDivergence
        
        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsSingleCriterion_IsTrue()
        {
            Assert.True(singleObjectivePopulation.RelativeDecisionVectorDivergence((DvDifference + 1E-9) / bestDv[1]));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsSingleCriterion_IsFalse()
        {
            Assert.False(singleObjectivePopulation.RelativeDecisionVectorDivergence(DvDifference / bestDv[1] / 2));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_MeetsMultipleCriteria_IsTrue()
        {
            var criteria = bestDv.Select(v => (DvDifference + 1E-9) / v).ToArray();
            Assert.True(singleObjectivePopulation.RelativeDecisionVectorDivergence(criteria));
        }

        [Fact]
        public void RelativeDecisionVectorDivergence_FailsMultipleCriteria_IsFalse()
        {
            var criteria = bestDv.Select(v => (DvDifference + 1E-9) / v / 2).ToArray();
            Assert.False(singleObjectivePopulation.RelativeDecisionVectorDivergence(criteria));
        }
        
        #endregion
    }
}