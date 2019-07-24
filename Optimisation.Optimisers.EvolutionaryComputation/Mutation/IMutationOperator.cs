using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// Interface for all mutation operators, that alter an existing <see cref="DecisionVector"/> in some way.
    /// </summary>
    public interface IMutationOperator
    {
        /// <summary>
        /// Mutates a decision vector.
        /// </summary>
        /// <param name="decisionVector">Current decision vector.</param>
        /// <returns>Mutated <see cref="DecisionVector"/>.</returns>
        DecisionVector Operate(DecisionVector decisionVector);
    }
}
