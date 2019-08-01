using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Optimization.TrustRegion;
using Optimisation.Base.Management;

namespace Optimisation.Optimisers.EvolutionaryComputation.ParentSelection
{
    /// <summary>
    /// A parent selection operator that selects two or more parents for recombination using tournaments.
    /// </summary>
    public class ParentSelectionTournament : Operator, IParentSelectionOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly bool alwaysReturnBest;
        private readonly int tournamentSize;
        
        /// <summary>
        /// Creates a tournament parent selection operator.
        /// </summary>
        /// <param name="tournamentSize">The number of potential parents which are entered into the tournament.</param>
        /// <param name="alwaysReturnBest"><see langword="true"/> if the best individual is always entered into the tournament.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the tournament size is less than 1.</exception>
        public ParentSelectionTournament(int tournamentSize = 40, bool alwaysReturnBest = false)
            : base($"Tournament (size {tournamentSize}" 
                   + (alwaysReturnBest ? ", keeping best)" : ")"))
        {
            if (tournamentSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(tournamentSize),
                    "The tournament size must be greater than zero.");
            this.tournamentSize = tournamentSize;

            this.alwaysReturnBest = alwaysReturnBest;
            
            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a subset of the population which wins a tournament.
        /// </summary>
        /// <param name="population">The <see cref="Population"/> to be selected from.</param>
        /// <param name="numberToSelect">The number of individuals to parents to select.</param>
        /// <returns>A list of <see cref="Individual"/>s selected.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when:
        /// 1) the number of parents to select is greater than the tournament size;
        /// 2) The population is smaller than the tournament size.
        /// </exception>
        public IEnumerable<Individual> Select(Population population, int numberToSelect)
        {
            if (numberToSelect > population.Count || numberToSelect > tournamentSize)
                throw new ArgumentOutOfRangeException(nameof(numberToSelect),
                    "The number to select cannot be greater than the size of the population or the size of the tournament.");

            var parents = new List<Individual>();
            
            var remainingEntrants = population.GetMemberList().ToList();

            if (alwaysReturnBest)
            {
                parents.Add(population.Best());
                remainingEntrants.Remove(population.Best());
            }

            while (parents.Count < numberToSelect)
            {
                var winner = runTournament(remainingEntrants);
                parents.Add(winner);
                remainingEntrants.Remove(winner);
            }
            
            return parents;
        }

        private Individual runTournament(IEnumerable<Individual> entrants)
        {
            // Will throw an error if tournament size is greater than entrants.Count()
            var ballotWinners = rngManager.GetLocations(entrants.Count(), tournamentSize, false, 1);
            // Return highest index, since the population is always sorted
            return entrants.ElementAt(ballotWinners.Min());
        }
    }
}