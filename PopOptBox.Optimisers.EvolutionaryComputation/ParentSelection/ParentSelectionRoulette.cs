using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Optimisers.EvolutionaryComputation.ParentSelection
{
    /// <summary>
    /// A parent selection operator that selects two or more parents for recombination using spins of a roulette wheel.
    /// </summary>
    public class ParentSelectionRoulette : Operator, IParentSelectionOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly bool alwaysReturnBest;

        /// <summary>
        /// Creates a roulette wheel parent selection operator.
        /// </summary>
        /// <param name="alwaysReturnBest"><see langword="true"/> if the best individual is always returned as one of the parents.</param>
        public ParentSelectionRoulette(bool alwaysReturnBest)
            : base("Roulette wheel" + (alwaysReturnBest ? " (keeping best)" : ""))
        {
            this.alwaysReturnBest = alwaysReturnBest;
            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a subset of the population which is selected based on spins of a roulette wheel,
        /// where the size of the selection probability for each individual is proportional to its Fitness.
        /// </summary>
        /// <remarks>
        /// The selection operator is corrected from the standard one to work with negative fitnesses and minimisation.
        /// </remarks>
        /// <param name="population">The <see cref="Population"/> to be selected from.</param>
        /// <param name="numberToSelect">The number of individuals to parents to select.</param>
        /// <returns>A list of <see cref="Individual"/>s selected.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when population is smaller than the number of individuals requested.</exception>
        public IEnumerable<Individual> Select(Population population, int numberToSelect)
        {
            if (numberToSelect > population.Count)
                throw new ArgumentOutOfRangeException(nameof(numberToSelect), 
                    "The population is too small to select this many individuals.");

            var negativeFitnessesAscending = getCorrectedFitnesses(population);

            var parents = new List<Individual>();
            var parentIndices = new List<int>();

            if (alwaysReturnBest)
            {
                parents.Add(population.Best());
                parentIndices.Add(0); // Best is always zero index
                negativeFitnessesAscending.RemoveAt(0);
            }

            var selectionProbability = getCumulativeSelectionProbabilities(negativeFitnessesAscending);

            // Make selection
            while (parents.Count < numberToSelect)
            {
                //Parent is selected based on random number generator
                //Find part of the roulette wheel the ball has landed in :)
                var newParentIdx = Array.FindIndex(selectionProbability, x => x > rngManager.Rng.NextDouble());

                var idxOffset = parentIndices.Count(i => i <= newParentIdx);

                parents.Add(population[newParentIdx + idxOffset]);
                parentIndices.Add(newParentIdx + idxOffset);

                negativeFitnessesAscending.RemoveAt(newParentIdx);
                selectionProbability = getCumulativeSelectionProbabilities(negativeFitnessesAscending);
            }

            return parents;
        }

        /// <summary>
        /// Fix because we are dealing with minimisation of Fitness:
        /// subtract a big enough value such that all values are strictly negative
        /// </summary>
        private static List<double> getCorrectedFitnesses(Population population)
        {
            var fitnessesAscending = population.GetMemberFitnesses();
            var maxFitness = fitnessesAscending.Last();
            var minFitness = fitnessesAscending.First();
            var range = maxFitness - minFitness;
            // Remove maxFitness so that all elements are negative.
            // Remove range so that even the worst element gets a relatively big chunk of probability,
            // but better elements get bigger chunks.
            return fitnessesAscending.Select(f => f - maxFitness - range).ToList();
        }

        private static double[] getCumulativeSelectionProbabilities(IReadOnlyList<double> correctedFitnesses)
        {
            var sumOfFitness = correctedFitnesses.Sum();
            var selectionProbability = new double[correctedFitnesses.Count];
            for (var i = 0; i < correctedFitnesses.Count; i++)
            {
                selectionProbability[i] = correctedFitnesses[i] / sumOfFitness;
                if (i != 0)
                    selectionProbability[i] += selectionProbability[i - 1];
            }

            return selectionProbability;
        }
    }
}