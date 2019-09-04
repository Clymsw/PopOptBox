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

        // Multi-objective
        private readonly Individual moTestInd;
        private readonly Individual indEqual;
        private readonly Individual indParetoDominant;
        private readonly Individual indParetoEqual;
        private readonly Individual indWrong;
        
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
            
            // Set up multi-objective population
            
            moTestInd = ObjectCreators.GetIndividual(new double[] { 0, 2 });
            moTestInd.SendForEvaluation();
            
            var solution1 = 0.2;
            var solution1Name = ObjectCreators.Solution_Key;
            var solution2 = 5.1;
            var solution2Name = ObjectCreators.Solution_Key + "2";
            var solution3 = 55.0;
            var solution3Name = ObjectCreators.Solution_Key + "3";
            
            moTestInd.SetProperty(solution1Name, solution1);
            moTestInd.SetProperty(solution2Name, solution2);
            moTestInd.SetProperty(solution3Name, solution3);
            
            indEqual = moTestInd.Clone();
            indParetoDominant = moTestInd.Clone();
            indParetoEqual = moTestInd.Clone();
            indWrong = moTestInd.Clone();
            
            indParetoDominant.SetProperty(solution1Name, solution1 - 0.1);
            indParetoEqual.SetProperty(solution1Name, solution1 - 0.1);
            indParetoEqual.SetProperty(solution2Name, solution2 + 0.1);
                
            moTestInd.SetSolution(solution1Name, solution2Name, solution3Name);
            indEqual.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoDominant.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoEqual.SetSolution(solution1Name, solution2Name, solution3Name);
            indWrong.SetSolution(solution1Name, solution2Name);
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
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherSolutionVectorIsDifferentLength_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => moTestInd.IsDominating(indWrong));
            Assert.Throws<InvalidOperationException>(() => indWrong.IsDominating(moTestInd));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsStrictlyBetterOnOneObjective_ReturnsTrue()
        {
            Assert.False(moTestInd.IsDominating(indParetoDominant));
            Assert.True(indParetoDominant.IsDominating(moTestInd));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsEqual_ReturnsFalse()
        {
            Assert.False(moTestInd.IsDominating(indEqual));
            Assert.False(indEqual.IsDominating(moTestInd));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsBetterOnOneAndWorseOnAnother_ReturnsFalse()
        {
            Assert.False(moTestInd.IsDominating(indParetoEqual));
            Assert.False(indParetoEqual.IsDominating(moTestInd));
        }
        
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