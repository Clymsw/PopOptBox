namespace PopOptBox.Base.Helpers
{
    /// <summary>
    /// Delegate functions for Solution Vector to Fitness conversion for single objectives
    /// <see cref="Management.Optimiser"/>.
    /// </summary>
    public static class SolutionToFitnessSingleObjective
    {
        /// <summary>
        /// Standard function for single objective minimisation.
        /// </summary>
        /// <param name="solution">The Solution Vector.</param>
        /// <returns>The Fitness.</returns>
        public static double Minimise(double[] solution)
        {
            //Optimisers should minimise by default!
            return solution[0];
        }

        /// <summary>
        /// Standard function for single objective maximisation.
        /// </summary>
        /// <param name="solution">The Solution Vector.</param>
        /// <returns>The Fitness.</returns>
        public static double Maximise(double[] solution)
        {
            //Optimisers should minimise by default!
            return -solution[0];
        }
    }
}