using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using PopOptBox.Base.PopulationCalculation;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Performs the Self-Adaptive Simulated Binary Crossover (SBX) as proposed by Jain and Deb (2011)
    /// </summary>
    public class CrossoverSimulatedBinarySelfAdaptive2 : Operator, IRecombinationOperator
    {
        private readonly Population population;
        private readonly CrossoverSimulatedBinary crossoverOperator;

        /// <summary>
        /// Constructs a crossover operator to perform self-adaptive parent- to mean-centric
        /// simulated binary crossover on real-valued decision vectors.
        /// </summary>
        /// <param name="population">
        /// The <see cref="Population"/> of the <see cref="EvolutionaryAlgorithm"/> to track during the optimisation.
        /// </param>
        /// <param name="eta">The expansion parameter for <see cref="CrossoverSimulatedBinary"/>.</param>
        public CrossoverSimulatedBinarySelfAdaptive2(Population population, int eta = 2) 
            : base($"Self-Adaptive (Parent- to Mean-Centric) Simulated Binary (eta = {eta.ToString()})")
        {
            this.population = population;
            crossoverOperator = new CrossoverSimulatedBinary(eta);
        }

        public DecisionVector Operate(params DecisionVector[] parents)
        {
            var lambda = getLambda();

            var parentsSum = Vector<double>.Build.DenseOfEnumerable(parents[0] + parents[1]);
            var parentsDifference = Vector<double>.Build.DenseOfEnumerable(parents[1] - parents[0]);

            var fakeParent1 = 0.5 * (parentsSum - lambda * parentsDifference);
            var fakeParent2 = 0.5 * (parentsSum + lambda * parentsDifference);
            
            return crossoverOperator.Operate(
                DecisionVector.CreateFromArray(parents[0].GetDecisionSpace(), fakeParent1), 
                DecisionVector.CreateFromArray(parents[0].GetDecisionSpace(), fakeParent2));
        }

        private double getLambda()
        {
            var centroid = Vector<double>.Build.DenseOfEnumerable(population.Centroid());

            var distances = population.GetMemberDecisionVectors()
                .Select(dv => Vector<double>.Build.DenseOfEnumerable(dv.Select(v => (double) v)))
                .Select(dv => (dv - centroid).L2Norm())
                .ToArray();

            return distances.First() / distances.Average();
        }
    }
}