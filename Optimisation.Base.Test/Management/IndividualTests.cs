using Optimisation.Base.Variables;
using System.Linq;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class IndividualTests
    {
        private const int Dims = 2;
        private const double MinBound = 0;
        private const double MaxBound = 2;
        
        private const string SolutionKey = "solution";
        private const string CloningKey = "cloneTest";

        private readonly double[] testVector;

        private readonly DecisionSpace space;
        private readonly Individual ind;

        public IndividualTests()
        {
            space = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinBound, MaxBound);
            
            testVector = new[] { 1, 1.5 };
            var dv = DecisionVector.CreateFromArray(space, testVector);
            
            ind = new Individual(dv);
        }

        [Fact]
        public void NewIndividual_StateIsNew()
        {
            Assert.Equal(IndividualStates.New, ind.State);
        }

        [Fact]
        public void NewIndividuals_HaveSameDecisionVector_AreEqual()
        {
            var space2 = DecisionSpace.CreateForUniformDoubleArray(
                Dims, MinBound, MaxBound);
            var vector2 = testVector.ToArray();
            var dv2 = DecisionVector.CreateFromArray(space2, vector2);
            var ind2 = new Individual(dv2);

            Assert.Equal(ind2, ind);
        }
        
        [Fact]
        public void NewIndividuals_HaveDifferentDecisionVector_AreNotEqual()
        {
            var vector2 = testVector.Select(i => i - 0.01).ToArray();
            var dv2 = DecisionVector.CreateFromArray(space, vector2);
            var ind2 = new Individual(dv2);

            Assert.NotEqual(ind2, ind);
        }
        
        [Fact]
        public void ClonedIndividuals_AreEqualButNotTheSame()
        {
            var ind1 = ind.Clone();

            Assert.Equal(ind1, ind);

            ind1.SetProperty(CloningKey, 1.2);
            ind1.SetProperty(SolutionKey, new[]{0.2, 5.1, 55});
            ind1.SetSolution(SolutionKey);
            ind1.SetScore(sol => sol);
            ind1.SetFitness(score => score[0]);

            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => ind.GetProperty<double>(CloningKey));
            Assert.NotEqual(ind.SolutionVector, ind1.SolutionVector);
            Assert.NotEqual(ind.Score, ind1.Score);
            Assert.NotEqual(ind.Fitness, ind1.Fitness);
        }

        
        [Fact]
        public void TwoIndividuals_HaveSameSolutionVector_AreEqual()
        {
            var vector2 = testVector.ToArray();
            var dv2 = DecisionVector.CreateFromArray(space, vector2);
            var ind2 = new Individual(dv2);
            
            var ind1 = ind.Clone();
            
            ind1.SetProperty(SolutionKey, new[] { 2.6 });
            ind1.SetSolution(SolutionKey);
            ind2.SetProperty(SolutionKey, new[] { 2.6 });
            ind2.SetSolution(SolutionKey);
            
            Assert.Equal(ind2, ind1);
        }

        [Fact]
        public void Individual_SolutionSettingWorks()
        {
            var ind1 = ind.Clone();

            var solution = new[] {0.2, 5.1, 55};
            ind1.SetProperty(SolutionKey, solution);
            ind1.SetSolution(SolutionKey);

            Assert.Equal(solution[0], ind1.SolutionVector.ElementAt(0));
            Assert.Equal(solution[1], ind1.SolutionVector.ElementAt(1));
            Assert.Equal(solution[2], ind1.SolutionVector.ElementAt(2));
        }
        
        [Fact]
        public void Individual_ScoreSettingWorks()
        {
            var ind1 = ind.Clone();

            var solution = new[] {0.2, 5.1, 55};
            ind1.SetProperty(SolutionKey, solution);
            ind1.SetSolution(SolutionKey);
            ind1.SetScore(sol => sol.Select(s => s * 2).ToArray());

            Assert.Equal(solution[0] * 2, ind1.Score.ElementAt(0));
            Assert.Equal(solution[1] * 2, ind1.Score.ElementAt(1));
            Assert.Equal(solution[2] * 2, ind1.Score.ElementAt(2));
        }
        
        [Fact]
        public void Individual_FitnessSettingWorks()
        {
            var ind1 = ind.Clone();

            var solution = new[] {0.2, 5.1, 55};
            ind1.SetProperty(SolutionKey, solution);
            ind1.SetSolution(SolutionKey);
            ind1.SetScore(sol => sol.Select(s => s * 2).ToArray());
            ind1.SetFitness(score => score[0]);

            Assert.Equal(solution[0] * 2, ind1.Fitness);
        }

    }
}
