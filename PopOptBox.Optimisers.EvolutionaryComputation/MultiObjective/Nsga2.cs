using System;
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
        private readonly bool[] minimise;
        private readonly int populationSize;
        private readonly IDominationSorter sorter;
        private const string CrowdingDistance = "Crowding Distance (temporary)";

        private readonly bool useReferencePoint;
        private double[] referencePoint;

        /// <summary>
        /// Construct the NSGA-II fitness calculation engine.
        /// </summary>
        /// <param name="minimise">For each objective, whether it should be minimised.</param>
        /// <param name="populationSize">The population size for the <see cref="Optimiser"/>.</param>
        /// <param name="sorter">The <see cref="IDominationSorter"/> to be used.</param>
        /// <param name="useReferencePoint">Whether to restrict Pareto Front value to a certain region. Default: false (standard NSGA-II).</param>
        /// <param name="referencePoint">The reference point to use (the worst acceptable value, not the best). Ignored if useReferencePoint = false.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the reference point provided has the wrong dimensionality.</exception>
        public Nsga2(IEnumerable<bool> minimise, int populationSize, IDominationSorter sorter,
            bool useReferencePoint = false, IEnumerable<double> referencePoint = null)
        {
            this.minimise = minimise.ToArray();
            this.populationSize = populationSize; 
            this.sorter = sorter ?? new FastNonDominatedSort();
            
            this.useReferencePoint = useReferencePoint;
            if (useReferencePoint)
            {
                if (referencePoint == null)
                    this.referencePoint = this.minimise.Select(i => double.NegativeInfinity).ToArray();
                else
                {
                    var refPoint = referencePoint.ToArray();
                    if (refPoint.Length != this.minimise.Length)
                        throw new ArgumentOutOfRangeException(nameof(referencePoint),
                            "reference point does not have the same number of dimensions as 'minimise'.");
                    this.referencePoint = refPoint;
                }
            }
            else
            {
                this.referencePoint = this.minimise.Select(m => m ? double.MaxValue : double.MinValue).ToArray();
            }
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
            
            if (useReferencePoint & double.IsInfinity(referencePoint.ElementAt(0)))
            {
                // Calculate a reference point
                for (var m = 0; m < inds[0].SolutionVector.Length; m++)
                {
                    // Use the worst value found for each objective, after the initial population generation.
                    referencePoint[m] = minimise[m]
                        ? inds.Max(i => i.SolutionVector.ElementAt(m))
                        : inds.Min(i => i.SolutionVector.ElementAt(m));
                }
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
                    ParetoFrontMetrics.AssignCrowdingDistance(currentParetoFront, CrowdingDistance,
                        minimise, referencePoint);
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