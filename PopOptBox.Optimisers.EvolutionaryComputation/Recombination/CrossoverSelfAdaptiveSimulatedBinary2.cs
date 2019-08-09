using System;
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
        
        public CrossoverSelfAdaptiveSimulatedBinary2(Population population) 
            : base("Self-Adaptive (Parent- to Mean-Centric) Simulated Binary")
        {
            this.population = population;
        }

        public DecisionVector Operate(params DecisionVector[] parents)
        {
            throw new NotImplementedException();
        }
    }
}