using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the Self-Adaptive Simulated Binary Crossover (SBX) as proposed by Jain and Deb (2011)
    /// </summary>
    public class CrossoverSelfAdaptiveSimulatedBinary2 : Operator, IRecombinationOperator
    {
        private Population population;
        
        /// <summary>
        /// Constructs a crossover operator to perform self-adaptive parent- to mean-centric
        /// simulated binary crossover on real-valued decision vectors.
        /// </summary>
        /// <param name="population">
        /// The <see cref="Population"/> from the <see cref="EvolutionaryAlgorithm"/> to track during the optimisation.
        /// </param>
        public CrossoverSelfAdaptiveSimulatedBinary2(Population population) 
            : base("Self-Adaptive (Parent- to Mean-Centric) Simulated Binary")
        {
            this.population = population;
        }

        public DecisionVector Operate(params DecisionVector[] parents)
        {
            // TODO - Use Mathnet.Numerics!
            var best = population.Best().DecisionVector.Select(v => (double) v).ToArray();
            var centroid = population
                .Select(i => i.DecisionVector.Select(v => (double) v))
                .Aggregate((a,b) => a.Select((k,l) => k + b.ElementAt(l)))
                .Select(x => x / population.Count);
            
            var dBest = best
            
            throw new NotImplementedException();
        }
    }
}