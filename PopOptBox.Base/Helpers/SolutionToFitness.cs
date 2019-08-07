namespace PopOptBox.Base.Helpers
{
    /// <summary>
    /// Delegate functions for Solution Vector to Fitness conversion, <see cref="Management.Optimiser"/>.
    /// </summary>
    public static class SolutionToFitness
    {
        /// <summary>
        /// Standard function for single objective minimisation.
        /// </summary>
        /// <param name="solution">The Solution Vector.</param>
        /// <returns>The Fitness.</returns>
        public static double SingleObjectiveMinimise(double[] solution)
        {
            //Optimisers should minimise by default!
            return solution[0];
        }

        /// <summary>
        /// Standard function for single objective maximisation.
        /// </summary>
        /// <param name="solution">The Solution Vector.</param>
        /// <returns>The Fitness.</returns>
        public static double SingleObjectiveMaximise(double[] solution)
        {
            //Optimisers should minimise by default!
            return -solution[0];
        }
    }
}