namespace Optimisation.Base.Helpers
{
    /// <summary>
    /// Helper functions for Solution Vector to Fitness delegates, <see cref="Management.Optimiser"/>.
    /// </summary>
    public static class SolutionToFitness
    {
        /// <summary>
        /// Standard function for single objective minimisation.
        /// </summary>
        /// <param name="solution">The solution vector.</param>
        /// <returns>The fitness.</returns>
        public static double SingleObjectiveMinimise(double[] solution)
        {
            //Optimisers should minimise by default!
            return solution[0];
        }

        /// <summary>
        /// Standard function for single objective maximisation.
        /// </summary>
        /// <param name="solution">The solution vector.</param>
        /// <returns>The fitness.</returns>
        public static double SingleObjectiveMaximise(double[] solution)
        {
            //Optimisers should minimise by default!
            return -solution[0];
        }
    }
}