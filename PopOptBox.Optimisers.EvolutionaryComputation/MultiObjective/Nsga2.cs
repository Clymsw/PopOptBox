using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base;
using PopOptBox.Base.FitnessCalculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.MultiObjectiveCalculation;

namespace PopOptBox.Optimisers.EvolutionaryComputation.MultiObjective
{
    /// <summary>
    /// Implements the NSGA-II multi-objective Evolutionary Algorithm as proposed by Deb et al. 2002
    /// </summary>
    public class Nsga2 : IFitnessCalculator
    {
        private bool[] minimise;
        private int populationSize;
        private IDominationSorter sorter;
        private const string CrowdingDistance = "Crowding Distance (temporary)";

        /// <summary>
        /// Construct the NSGA-II fitness calculation engine.
        /// </summary>
        /// <param name="minimise"></param>
        /// <param name="populationSize"></param>
        /// <param name="sorter"></param>
        public Nsga2(IEnumerable<bool> minimise, int populationSize, IDominationSorter sorter = null)
        {
            this.minimise = minimise.ToArray();
            this.populationSize = populationSize; 
            this.sorter = sorter ?? new FastNonDominatedSort();
        }
        
        /// <summary>
        /// Performs the fitness calculation based on:
        /// 1) Non-dominated sorting into Pareto Fronts;
        /// 2) Crowding distance assignment and comparison
        /// </summary>
        /// <remarks>
        /// If the number of individuals provided is less than the population size provided in the constructor,
        /// then only a simple fitness assignment is performed (assumes this is used during initial population
        /// creation: <see cref="EvolutionaryAlgorithm"/>). 
        /// </remarks>
        /// <param name="individuals">All individuals to be considered for fitness calculation.</param>
        public void CalculateAndAssignFitness(IEnumerable<Individual> individuals)
        {
            var inds = individuals as Individual[] ?? individuals.ToArray();
            
            if (inds.Length <= populationSize)
            {
                // Don't bother with Pareto Front calculation, it's too time consuming.
                // Just assign sum of ranks on each objective.
                for (var m = 0; m < inds[0].SolutionVector.Length; m++)
                {
                    var individualsOrderedByThisObjective = inds.OrderBy(i => i.SolutionVector.ElementAt(m) * (minimise[m] ? 1 : -1)).ToArray();
                    for (int o = 0; o < individualsOrderedByThisObjective.Count(); o++)
                    {
                        individualsOrderedByThisObjective[o].SetFitness(o + individualsOrderedByThisObjective[o].Fitness);
                    }
                }
                return;
            }
            
            // Calculate Pareto Fronts
            sorter.PerformSort(inds, minimise);

            // Go through, assigning fitness
            var paretoFront = 1;
            var currentParetoFront = inds.Where(i => i.GetProperty<int>(OptimiserPropertyNames.ParetoFront) == paretoFront).ToArray();
            var individualsAssessed = 0;
            while (currentParetoFront.Length > 0)
            {
                foreach (var ind in currentParetoFront)
                {
                    ind.SetFitness(paretoFront);
                }
                
                if (individualsAssessed < populationSize && individualsAssessed + currentParetoFront.Length > populationSize)
                {
                    // We're in the Pareto Front which partially overlaps with the population size.
                    // Perform crowding distance assignment
                    MultiObjectiveMetrics.AssignCrowdingDistance(currentParetoFront, CrowdingDistance);
                    var sortedParetoFront = currentParetoFront
                        .OrderByDescending(i => i.GetProperty<double>(CrowdingDistance))
                        .ToArray();
                    var crowdingComparison = 0.0;
                    for (int i = 1; i < sortedParetoFront.Length; i++)
                    {
                        if (sortedParetoFront[i - 1].GetProperty<double>(CrowdingDistance) >
                            sortedParetoFront[i].GetProperty<double>(CrowdingDistance))
                            crowdingComparison += 1.0 / (currentParetoFront.Length + 10.0); // Small enough that we won't end up more than 1 in total...
                        sortedParetoFront[i].SetFitness(sortedParetoFront[i].Fitness + crowdingComparison);
                    }
                }

                individualsAssessed += currentParetoFront.Length;
                
                // Get next Pareto Front
                paretoFront++;
                currentParetoFront = inds.Where(i => i.GetProperty<int>(OptimiserPropertyNames.ParetoFront) == paretoFront).ToArray();
            }
        }
    }
}