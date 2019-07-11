namespace Optimisation.Base.Helpers
{
    public static class SolutionToScore
    {
        /// <summary>
        /// Standard function for single objective optimisation
        /// </summary>
        /// <param name="solution">The solution vector.</param>
        /// <returns>The score vector.</returns>
        public static double[] SingleObjective(double[] solution)
        {
            // Just pass through.
            return solution;
        }
    }
}