using System.Collections.Generic;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation.Recombination
{
    /// <summary>
    /// Recombination operator, where multiple parent decision vectors are used to create an 'offspring'.
    /// </summary>
    public interface IMultiParentRecombinationOperator
    {
        /// <summary>
        /// Performs the recombination.
        /// </summary>
        /// <param name="parents">The list of parent decision vectors.</param>
        /// <returns>A new <see cref="DecisionVector"/>.</returns>
        DecisionVector Operate(IEnumerable<DecisionVector> parents);
    }
}
