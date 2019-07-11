namespace Optimisation.Base.Helpers
{
    public static class Penalty
    {
        /// <summary>
        /// Applies a sudden "wall" to the optimisation when the DV is outside the legal range
        /// </summary>
        /// <param name="solVector">Solution vector is not used</param>
        /// <returns><see cref="double.MaxValue"/></returns>
        public static double DeathPenalty(double[] solVector)
        {
            // Optimisers minimise, therefore this is the worst possible value.
            return double.MaxValue;
        }
    }
}