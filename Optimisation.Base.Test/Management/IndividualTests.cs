using System;
using System.Linq;
using Optimisation.Base.Test.Helpers;
using Xunit;

namespace Optimisation.Base.Management.Test
{
    public class IndividualTests
    {
        private const string Cloning_Key = "cloneTest";

        private readonly double[] testVector;

        private readonly Individual ind;

        public IndividualTests()
        {
            testVector = new double[] { 0, 2 };
            var dv = ObjectCreators.GetDecisionVector(testVector);
            ind = new Individual(dv);
        }

        [Fact]
        public void NewIndividual_StateIsNew()
        {
            Assert.Equal(IndividualState.New, ind.State);
        }

        [Fact]
        public void NewIndividuals_HaveSameDecisionVector_AreEqual()
        {
            var vector2 = testVector.ToArray();
            var dv2 = ObjectCreators.GetDecisionVector(vector2);
            var ind2 = new Individual(dv2);

            Assert.Equal(ind2, ind);
        }
        
        [Fact]
        public void NewIndividuals_HaveDifferentDecisionVectorValues_AreNotEqual()
        {
            var vector2 = testVector.Select(i => i + 0.0001).ToArray();
            var dv2 = ObjectCreators.GetDecisionVector(vector2);
            var ind2 = new Individual(dv2);

            Assert.NotEqual(ind2, ind);
        }
        
        [Fact]
        public void NewIndividuals_HaveDifferentDecisionSpace_AreNotEqual()
        {
            var vector2 = testVector.Select(i => (int)i).ToArray();
            var dv2 = ObjectCreators.GetDecisionVector(vector2);
            var ind2 = new Individual(dv2);

            Assert.NotEqual(ind2, ind);
        }
        
        [Fact]
        public void ClonedIndividuals_AreEqualButNotTheSame()
        {
            var ind1 = ind.Clone();

            Assert.Equal(ind1, ind);

            ind1.SetProperty(Cloning_Key, 1.2);
            ind1.SetProperty(ObjectCreators.Solution_Key, new[]{0.2, 5.1, 55});
            ind1.SetSolution(ObjectCreators.Solution_Key);
            ind1.SetFitness(s => s[0]);

            Assert.Throws<System.ArgumentOutOfRangeException>(
                () => ind.GetProperty<double>(Cloning_Key));
            Assert.NotEqual(ind.SolutionVector, ind1.SolutionVector);
            Assert.NotEqual(ind.Fitness, ind1.Fitness);
        }

        
        [Fact]
        public void TwoIndividuals_HaveSameSolutionVector_AreEqual()
        {
            var vector2 = testVector.ToArray();
            var dv2 = ObjectCreators.GetDecisionVector(vector2);
            var ind2 = new Individual(dv2);
            
            ind.SetProperty(ObjectCreators.Solution_Key, new[] { 2.6 });
            ind.SetSolution(ObjectCreators.Solution_Key);
            ind2.SetProperty(ObjectCreators.Solution_Key, new[] { 2.6 });
            ind2.SetSolution(ObjectCreators.Solution_Key);
            
            Assert.Equal(ind2, ind);
        }

        [Fact]
        public void Individual_SolutionSettingWorks()
        {
            var solution = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution);
            ind.SetSolution(ObjectCreators.Solution_Key);

            Assert.Equal(solution[0], ind.SolutionVector.ElementAt(0));
            Assert.Equal(solution[1], ind.SolutionVector.ElementAt(1));
            Assert.Equal(solution[2], ind.SolutionVector.ElementAt(2));
        }
        
        [Fact]
        public void Individual_FitnessSettingWorks()
        {
            var solution = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution);
            ind.SetSolution(ObjectCreators.Solution_Key);
            ind.SetFitness(s => s[0] * 2);

            Assert.Equal(solution[0] * 2, ind.Fitness);
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherSolutionVectorIsDifferentLength_Throws()
        {
            var solution = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution);
            ind.SetSolution(ObjectCreators.Solution_Key);
            
            var otherInd = ind.Clone();
            var solution2 = new[] {0.1, 54.9};
            otherInd.SetProperty(ObjectCreators.Solution_Key, solution2);
            otherInd.SetSolution(ObjectCreators.Solution_Key);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => ind.IsDominatedBy(otherInd));
            Assert.Throws<ArgumentOutOfRangeException>(() => otherInd.Dominates(ind));
        }

        [Fact]
        public void Dominates_IsDominatedBy_OtherIsStrictlyBetter_ReturnsTrue()
        {
            var solution = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution);
            ind.SetSolution(ObjectCreators.Solution_Key);
            
            var otherInd = ind.Clone();
            var solution2 = new[] {0.1, 5.0, 54.9};
            otherInd.SetProperty(ObjectCreators.Solution_Key, solution2);
            otherInd.SetSolution(ObjectCreators.Solution_Key);
            
            Assert.True(ind.IsDominatedBy(otherInd));
            Assert.False(ind.Dominates(otherInd));
            Assert.True(otherInd.Dominates(ind));
            Assert.False(otherInd.IsDominatedBy(ind));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OtherIsEqual_ReturnsFalse()
        {
            var solution = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution);
            ind.SetSolution(ObjectCreators.Solution_Key);
            
            var otherInd = ind.Clone();
            otherInd.SetProperty(ObjectCreators.Solution_Key, solution);
            otherInd.SetSolution(ObjectCreators.Solution_Key);
            
            Assert.False(ind.IsDominatedBy(otherInd));
            Assert.False(ind.Dominates(otherInd));
            Assert.False(otherInd.Dominates(ind));
            Assert.False(otherInd.IsDominatedBy(ind));
        }
        
        [Fact]
        public void Dominates_IsDominatedBy_OnParetoFront_ReturnsFalse()
        {
            var solution = new[] {0.2, 5.1, 55};
            ind.SetProperty(ObjectCreators.Solution_Key, solution);
            ind.SetSolution(ObjectCreators.Solution_Key);
            
            var otherInd = ind.Clone();
            var solution2 = new[] {0.1, 5.2, 55.0};
            otherInd.SetProperty(ObjectCreators.Solution_Key, solution2);
            otherInd.SetSolution(ObjectCreators.Solution_Key);
            
            Assert.False(ind.IsDominatedBy(otherInd));
            Assert.False(ind.Dominates(otherInd));
            Assert.False(otherInd.Dominates(ind));
            Assert.False(otherInd.IsDominatedBy(ind));
        }
    }
}
