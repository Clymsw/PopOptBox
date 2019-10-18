using System;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.MultiObjectiveCalculation.Test
{
    public class IndividualMetricsTests
    {
        private readonly Individual moTestInd;
        private readonly Individual indEqual;
        private readonly Individual indParetoDominant;
        private readonly Individual indParetoEqual1;
        private readonly Individual indWrong;

        private readonly bool[] minimise;
        
        public IndividualMetricsTests()
        {
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
            indParetoEqual1 = moTestInd.Clone();
            indWrong = moTestInd.Clone();
            
            indParetoDominant.SetProperty(solution1Name, solution1 - 0.1);
            indParetoEqual1.SetProperty(solution1Name, solution1 - 0.1);
            indParetoEqual1.SetProperty(solution2Name, solution2 + 0.1);
            
            minimise = new[] {true, true, true};
            
            moTestInd.SetSolution(solution1Name, solution2Name, solution3Name);
            indEqual.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoDominant.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoEqual1.SetSolution(solution1Name, solution2Name, solution3Name);
            indWrong.SetSolution(solution1Name, solution2Name);
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherSolutionVectorIsDifferentLength_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => moTestInd.IsDominating(indWrong, minimise));
            Assert.Throws<InvalidOperationException>(() => indWrong.IsDominating(moTestInd, minimise));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsStrictlyBetterOnOneObjective_ReturnsTrue()
        {
            Assert.False(moTestInd.IsDominating(indParetoDominant, minimise));
            Assert.True(indParetoDominant.IsDominating(moTestInd, minimise));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsEqual_ReturnsFalse()
        {
            Assert.False(moTestInd.IsDominating(indEqual, minimise));
            Assert.False(indEqual.IsDominating(moTestInd, minimise));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsBetterOnOneAndWorseOnAnother_ReturnsFalse()
        {
            Assert.False(moTestInd.IsDominating(indParetoEqual1, minimise));
            Assert.False(indParetoEqual1.IsDominating(moTestInd, minimise));
        }
    }
}