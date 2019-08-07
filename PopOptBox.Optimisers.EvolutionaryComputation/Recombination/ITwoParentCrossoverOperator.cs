using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Crossover operator, which 'breeds' two decision vectors together to create an 'offspring'.
    /// </summary>
    public interface ITwoParentCrossoverOperator
    {
        /// <summary>
        /// Performs the crossover operation.
        /// </summary>
        /// <param name="firstParent">First parent for the crossover.</param>
        /// <param name="secondParent">Second parent for the crossover.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        DecisionVector Operate(DecisionVector firstParent, DecisionVector secondParent);
    }
}
