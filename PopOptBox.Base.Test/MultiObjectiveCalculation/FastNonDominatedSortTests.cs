using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;
using PopOptBox.Base.Test.Helpers;
using Xunit;

namespace PopOptBox.Base.MultiObjectiveCalculation.Test
{
    public class FastNonDominatedSortTests
    {
        private readonly List<Individual> individuals;
        private readonly bool[] minimise;
        
        public FastNonDominatedSortTests()
        {
            var solution1Name = ObjectCreators.Solution_Key;
            var solution2Name = ObjectCreators.Solution_Key + "2";
            
            individuals = new List<Individual>
            {
                ObjectCreators.GetIndividual(new double[] {0, 3}), // F1
                ObjectCreators.GetIndividual(new double[] {1, 3}), // F2
                ObjectCreators.GetIndividual(new double[] {1, 2}), // F1
                ObjectCreators.GetIndividual(new double[] {2, 2}), // F2
                ObjectCreators.GetIndividual(new double[] {2, 1}), // F1
                ObjectCreators.GetIndividual(new double[] {3, 2}), // F3
                ObjectCreators.GetIndividual(new double[] {3, 1}), // F2
                ObjectCreators.GetIndividual(new double[] {3, 0}), // F1
                ObjectCreators.GetIndividual(new double[] {4, 1})  // F3
            };

            minimise = new[] {true, true};
            
            foreach (var ind in individuals)
            {
                ind.SendForEvaluation();
                ind.SetProperty(solution1Name, ind.DecisionVector[0]);
                ind.SetProperty(solution2Name, ind.DecisionVector[1]);
                ind.SetSolution(solution1Name, solution2Name);
            }
        }

        [Fact]
        public void PerformSort_GetsParetoFrontsCorrect()
        {
            var sorter = new FastNonDominatedSort();
            
            sorter.PerformSort(individuals, minimise);

            Assert.True(individuals.ElementAt(0)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 1);
            Assert.True(individuals.ElementAt(1)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 2);
            Assert.True(individuals.ElementAt(2)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 1);
            Assert.True(individuals.ElementAt(3)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 2);
            Assert.True(individuals.ElementAt(4)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 1);
            Assert.True(individuals.ElementAt(5)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 3);
            Assert.True(individuals.ElementAt(6)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 2);
            Assert.True(individuals.ElementAt(7)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 1);
            Assert.True(individuals.ElementAt(8)
                            .GetProperty<int>(OptimiserPropertyNames.ParetoFront) == 3);
        }
        
        [Fact]
        public void ManyIndividuals_AllocatesParetoFrontToAll()
        {
            var rng = new System.Random();

            var shouldMinimise = new[] { true, true, true };
            var inds = new List<Individual>();
            for (var i = 0; i < 200; i++)
            {
                inds.Add(ObjectCreators.GetIndividual(new[] { rng.NextDouble(), rng.NextDouble(), rng.NextDouble() }));
            }

            foreach (var ind in inds)
            {
                ind.SendForEvaluation();
                ind.SetProperty("sol1", ind.DecisionVector[0]);
                ind.SetProperty("sol2", ind.DecisionVector[1]);
                ind.SetProperty("sol3", ind.DecisionVector[2]);
                ind.SetSolution("sol1", "sol2", "sol3");
            }

            var sorter = new FastNonDominatedSort();

            sorter.PerformSort(inds, shouldMinimise);

            Assert.True(inds.All(i => i.GetProperty<int>(OptimiserPropertyNames.ParetoFront) > 0));
        }
    }
}