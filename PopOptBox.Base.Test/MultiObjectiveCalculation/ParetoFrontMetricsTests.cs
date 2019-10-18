using System;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.MultiObjectiveCalculation.Test
{
    public class ParetoFrontMetricsTests
    {
        private const double WorstSolution1 = 0.2;
        private const double WorstSolution2 = 5.1;
        private const double WorstSolution3 = 55.0;
        private readonly Individual moTestInd;
        private readonly Individual indParetoEqual1;
        private readonly Individual indParetoEqual2;
        private readonly Individual indParetoEqual3;
        private readonly bool[] minimise;

        public ParetoFrontMetricsTests()
        {
            // Set up multi-objective population
            
            moTestInd = ObjectCreators.GetIndividual(new double[] { 0, 2 });
            moTestInd.SendForEvaluation();
            
            var solution1Name = ObjectCreators.Solution_Key;
            var solution2Name = ObjectCreators.Solution_Key + "2";
            var solution3Name = ObjectCreators.Solution_Key + "3";
            
            moTestInd.SetProperty(solution1Name, WorstSolution1 - 0.01); // 3
            moTestInd.SetProperty(solution2Name, WorstSolution2 - 0.18); // 2
            moTestInd.SetProperty(solution3Name, WorstSolution3 - 0.1); // 1
            
            indParetoEqual1 = moTestInd.Clone();
            indParetoEqual2 = moTestInd.Clone();
            indParetoEqual3 = moTestInd.Clone();

            indParetoEqual1.SetProperty(solution1Name, WorstSolution1 - 0.1); // 1
            indParetoEqual1.SetProperty(solution2Name, WorstSolution2); // 4
            indParetoEqual1.SetProperty(solution3Name, WorstSolution3 - 0.09); // 2
            indParetoEqual2.SetProperty(solution1Name, WorstSolution1 - 0.09); // 2
            indParetoEqual2.SetProperty(solution2Name, WorstSolution2 - 0.02); // 3
            indParetoEqual2.SetProperty(solution3Name, WorstSolution3 - 0.01); // 3
            indParetoEqual3.SetProperty(solution1Name, WorstSolution1); // 4
            indParetoEqual3.SetProperty(solution2Name, WorstSolution2 - 0.2); // 1
            indParetoEqual3.SetProperty(solution3Name, WorstSolution3); // 4
            
            minimise = new[] {true, true, true};
            
            moTestInd.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoEqual1.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoEqual2.SetSolution(solution1Name, solution2Name, solution3Name);
            indParetoEqual3.SetSolution(solution1Name, solution2Name, solution3Name);
        }
        
        [Fact]
        public void AssignCrowdingDistance_InfiniteReferencePoint_Works()
        {
            ParetoFrontMetrics.AssignCrowdingDistance(new[]
                    {moTestInd, indParetoEqual1, indParetoEqual2, indParetoEqual3},
                "crowdingDistance",
                minimise,
                moTestInd.SolutionVector.Select(i => double.MaxValue));
            
            Assert.True(moTestInd.GetProperty<double>("crowdingDistance") > 1e300);
            Assert.True(indParetoEqual1.GetProperty<double>("crowdingDistance") > 1e300);
            // Crowding distance should be 0.09/0.1 + 0.18/0.2 + 0.09/0.1
            Assert.True(Math.Abs(indParetoEqual2.GetProperty<double>("crowdingDistance") - 2.7) < 1e-6);
            Assert.True(indParetoEqual3.GetProperty<double>("crowdingDistance") > 1e300);
        }
        
        [Fact]
        public void AssignCrowdingDistance_OutsideReferencePoint_Works()
        {
            ParetoFrontMetrics.AssignCrowdingDistance(new[]
                    {moTestInd, indParetoEqual1, indParetoEqual2, indParetoEqual3},
                "crowdingDistance",
                minimise,
                new[]{WorstSolution1 + 0.1, WorstSolution2 + 0.1, WorstSolution3 + 0.1});
            
            // Best for objective 3
            Assert.True(moTestInd.GetProperty<double>("crowdingDistance") > 1e300);
            // Best for objective 1
            Assert.True(indParetoEqual1.GetProperty<double>("crowdingDistance") > 1e300);
            // Crowding distance should be 0.09/0.1 + 0.18/0.2 + 0.09/0.1
            Assert.True(Math.Abs(indParetoEqual2.GetProperty<double>("crowdingDistance") - 2.7) < 1e-6);
            // Best for objective 2
            Assert.True(indParetoEqual3.GetProperty<double>("crowdingDistance") > 1e300);
        }
        
        [Fact]
        public void AssignCrowdingDistance_InsideReferencePoint_Works()
        {
            ParetoFrontMetrics.AssignCrowdingDistance(new[]
                    {moTestInd, indParetoEqual1, indParetoEqual2, indParetoEqual3},
                "crowdingDistance",
                minimise,
                new[]{WorstSolution1 + 0.1, WorstSolution2 - 0.02, WorstSolution3 + 0.1});
            
            // Best for objective 3
            Assert.True(moTestInd.GetProperty<double>("crowdingDistance") > 1e300);
            // Out of bounds on objective 2
            Assert.Equal(0.0, indParetoEqual1.GetProperty<double>("crowdingDistance"));
            // Crowding distance should be 0.09/0.1 + 0.16/0.18 + 0.09/0.1
            Assert.True(Math.Abs(indParetoEqual2.GetProperty<double>("crowdingDistance") - 2.68888889) < 1e-6);
            // Best for objective 2
            Assert.True(indParetoEqual3.GetProperty<double>("crowdingDistance") > 1e300);
        }
        
    }
}