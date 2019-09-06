using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base;
using PopOptBox.Base.Calculation;
using PopOptBox.Base.Management;
using PopOptBox.Optimisers.EvolutionaryComputation.MultiObjective;
using Xunit;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Test.MultiObjective
{
    public class Nsga2Tests
    {
        private readonly List<Individual> individuals;
        
        public Nsga2Tests()
        {
            var solution1Name = "solution1";
            var solution2Name = "solution2";
            
            individuals = Helpers.CreateNewIndividualsFromArray(new[]
                {
                    new double[] {0, 3}, // F1
                    new double[] {1, 3}, // F2
                    new double[] {1, 2}, // F1
                    new double[] {2, 2}, // F2
                    new double[] {2, 1}, // F1
                    new double[] {3, 2}, // F3
                    new double[] {3, 1}, // F2
                    new double[] {3, 0}, // F1
                    new double[] {4, 1}  // F3
                });

            foreach (var ind in individuals)
            {
                ind.SendForEvaluation();
                ind.SetProperty(solution1Name, ind.DecisionVector[0]);
                ind.SetProperty(solution2Name, ind.DecisionVector[1]);
                ind.SetSolution(solution1Name, solution2Name);
            }
        }

        [Fact]
        public void CalculateAndAssignFitness_PopulationSize6_CalculatesFitnessesCorrectly()
        {
            var nsga2 = new Nsga2(new bool[2], 6, new FastNonDominatedSort());
            
            nsga2.CalculateAndAssignFitness(individuals);
            
            Assert.Equal(1.0, individuals.ElementAt(0).Fitness);
            Assert.True(individuals.ElementAt(1).Fitness >= 2 && individuals.ElementAt(0).Fitness < 3);
            Assert.Equal(1.0, individuals.ElementAt(2).Fitness);
            Assert.True(individuals.ElementAt(3).Fitness >= 2 && individuals.ElementAt(0).Fitness < 3);
            Assert.Equal(1.0, individuals.ElementAt(4).Fitness);
            Assert.Equal(3.0, individuals.ElementAt(5).Fitness);
            Assert.True(individuals.ElementAt(6).Fitness >= 2 && individuals.ElementAt(0).Fitness < 3);
            Assert.Equal(1.0, individuals.ElementAt(7).Fitness);
            Assert.Equal(3.0, individuals.ElementAt(8).Fitness);
        }
        
        [Fact]
        public void CalculateAndAssignFitness_PopulationSize7_CalculatesFitnessesCorrectly()
        {
            var nsga2 = new Nsga2(new bool[2], 7, new FastNonDominatedSort());
            
            nsga2.CalculateAndAssignFitness(individuals);
            
            // Population size is exactly same size as front 1 + front 2.
            Assert.Equal(1.0, individuals.ElementAt(0).Fitness);
            Assert.Equal(2.0, individuals.ElementAt(1).Fitness);
            Assert.Equal(1.0, individuals.ElementAt(2).Fitness);
            Assert.Equal(2.0, individuals.ElementAt(3).Fitness);
            Assert.Equal(1.0, individuals.ElementAt(4).Fitness);
            Assert.Equal(3.0, individuals.ElementAt(5).Fitness);
            Assert.Equal(2.0, individuals.ElementAt(6).Fitness);
            Assert.Equal(1.0, individuals.ElementAt(7).Fitness);
            Assert.Equal(3.0, individuals.ElementAt(8).Fitness);
        }
    }
}