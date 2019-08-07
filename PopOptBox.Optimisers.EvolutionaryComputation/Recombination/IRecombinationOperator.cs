using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Recombination (or crossover) operator, which 'breeds' parent decision vectors together to create an 'offspring'.
    /// </summary>
    public interface IRecombinationOperator
    {
        /// <summary>
        /// Performs the recombination (crossover) operation.
        /// </summary>
        /// <param name="parents">Parents for the recombination.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        DecisionVector Operate(params DecisionVector[] parents);
    }
}
