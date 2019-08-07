using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Mutation
{
    /// <summary>
    /// Interface for all mutation operators, that alter an existing <see cref="DecisionVector"/> in some way.
    /// </summary>
    public interface IMutationOperator
    {
        /// <summary>
        /// Mutates a Decision Vector.
        /// </summary>
        /// <param name="decisionVector">Current Decision Vector.</param>
        /// <returns>Mutated <see cref="DecisionVector"/>.</returns>
        DecisionVector Operate(DecisionVector decisionVector);
    }
}
