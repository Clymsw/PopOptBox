using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// A mutation operator for <see cref="VariableContinuous"/> <see cref="DecisionVector"/> elements, which can add a random number drawn from a Normal distribution.
    /// </summary>
    public class MutationAddRandomNumber : Operator, IMutationOperator
    {
        private readonly RandomNumberManager rngManager;
        private readonly double normalStandardDeviation;
        private readonly double mutationProbability;
        private readonly int maximumNumberOfMutations;

        /// <summary>
        /// Constructs a mutation operator that adds a random number from a Normal distribution to zero or more elements in the <see cref="DecisionVector"/>.
        /// </summary>
        /// <param name="normalStandardDeviation">The standard deviation of the Normal distribution around zero.</param>
        /// <param name="mutationProbability">The probability that any mutation will occur.</param>
        /// <param name="maximumNumberOfMutations">The maximum number of times a mutation should be tried.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the input values are illegal.</exception>
        public MutationAddRandomNumber(double normalStandardDeviation, double mutationProbability, int maximumNumberOfMutations)
            : base($"Add random number from N(0,{Math.Pow(normalStandardDeviation, 2).ToString("F2", CultureInfo.InvariantCulture)}) " +
                  $"to up to {maximumNumberOfMutations} locations " +
                  $"with chance {mutationProbability.ToString("F2", CultureInfo.InvariantCulture)}")
        {
            if (normalStandardDeviation <= 0)
                throw new ArgumentOutOfRangeException(nameof(normalStandardDeviation), "Mutation variance must be greater than 0.");
            this.normalStandardDeviation = normalStandardDeviation;

            if (mutationProbability < 0 || mutationProbability > 1)
                throw new ArgumentOutOfRangeException(nameof(mutationProbability), "Mutation probability must be a value between 0 and 1.");
            this.mutationProbability = mutationProbability;

            if (maximumNumberOfMutations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumNumberOfMutations), "Maximum number of mutations must be greater than 0.");
            this.maximumNumberOfMutations = maximumNumberOfMutations;

            rngManager = new RandomNumberManager();
        }

        /// <summary>
        /// Gets a new decision vector with continuous elements have potentially been mutated.
        /// </summary>
        /// <param name="decisionVector">The existing decision vector.</param>
        /// <returns>A new decision vector.</returns>
        /// <exception cref="ArgumentException">Thrown when decision vector is zero length or has no <seealso cref="VariableContinuous"/> elements.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when mutated values are not valid in the <see cref="DecisionSpace"/>.</exception>
        public DecisionVector Operate(DecisionVector decisionVector)
        {
            var oldVector = decisionVector.GetContinuousElements();

            if (oldVector.Vector.Count == 0)
                throw new ArgumentException("Decision Vector must have continuous elements",
                    nameof(decisionVector)); 
            
            var locationsToMutate = rngManager.GetMutationLocations(oldVector, maximumNumberOfMutations, true, mutationProbability);

            var newDv = new object[decisionVector.Vector.Count];
            var newDs = decisionVector.GetDecisionSpace();
            var offset = 0;
            for (var i = 0; i < decisionVector.Vector.Count; i++)
            {
                newDv[i] = decisionVector.Vector.ElementAt(i);

                // If variable is not continuous, just copy
                if (newDs.Dimensions.ElementAt(i).GetType() != typeof(VariableContinuous))
                {
                    offset++;
                    continue;
                }

                // Variable is continuous - it may be mutated multiple times.
                var numTimesToMutate = locationsToMutate.Count(l => l == (i + offset));

                for (var j = 0; j < numTimesToMutate; j++)
                {
                    var randomValue = Normal.Sample(rngManager.Rng, 0, normalStandardDeviation);
                    newDv[i] = (double)newDv[i] + randomValue;
                }
            }
            return DecisionVector.CreateFromArray(newDs, newDv);
        }
    }
}
