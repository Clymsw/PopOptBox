using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Optimization.TrustRegion;
using Optimisation.Base.Management;

namespace Optimisation.Optimisers.EvolutionaryComputation.ParentSelection
{
    /// <summary>
    /// A parent selection operator that selects two or more parents for recombination using a tournament.
    /// </summary>
    public class ParentSelectionTournament : Operator, IParentSelectionOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly bool alwaysUseBest;
        private readonly int tournamentSize;
        
        /// <summary>
        /// Creates a tournament parent selection operator.
        /// </summary>
        /// <param name="tournamentSize">The number of potential parents which are entered into the tournament.</param>
        /// <param name="alwaysUseBest"><see langword="true"/> if the best individual is always entered into the tournament.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the tournament size is not greater than zero.</exception>
        public ParentSelectionTournament(int tournamentSize = 40, bool alwaysUseBest = false)
            : base($"Tournament (size {tournamentSize}" 
                   + (alwaysUseBest ? ", keeping best)" : ")"))
        {
            if (tournamentSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(tournamentSize),
                    "The tournament size must be greater than zero.");
            this.tournamentSize = tournamentSize;

            this.alwaysUseBest = alwaysUseBest;
            
            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="population"></param>
        /// <param name="numberToSelect"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<Individual> Select(Population population, int numberToSelect)
        {
            if (numberToSelect > population.Count || numberToSelect > tournamentSize)
                throw new ArgumentOutOfRangeException(nameof(numberToSelect),
                    "The number to select cannot be greater than the size of the population or the size of the tournament.");

            var parents = new List<Individual>();
            
            var remainingEntrants = population.GetMemberList().ToList();

            if (alwaysUseBest)
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
            return entrants.First();
            //TODO!
        }
    }
}