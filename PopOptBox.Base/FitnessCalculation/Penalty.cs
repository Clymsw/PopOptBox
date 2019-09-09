using PopOptBox.Base.Variables;

namespace PopOptBox.Base.FitnessCalculation
{
    /// <summary>
    /// Delegate functions for penalties, <see cref="Management.Optimiser"/>.
    /// </summary>
    public static class Penalty
    {
        /// <summary>
        /// Applies a sudden "wall" to the optimisation when the DV is outside the legal range
        /// </summary>
        /// <param name="decisionVector">Decision Vector is not used.</param>
        /// <returns><see cref="double.MaxValue"/></returns>
        public static double DeathPenalty(DecisionVector decisionVector = null)
        {
            // Optimisers minimise, therefore this is the worst possible value.
            return double.MaxValue;
        }
    }
}