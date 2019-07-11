namespace Optimisation.Base.Helpers
{
    public static class ScoreToFitness
    {
        /// <summary>
        /// Standard function for single objective minimisation
        /// </summary>
        /// <param name="score">The score vector.</param>
        /// <returns>The fitness.</returns>
        public static double SingleObjectiveMinimise(double[] score)
        {
            //Optimisers should minimise by default!
            return score[0];
        }

        /// <summary>
        /// Standard function for single objective maximisation
        /// </summary>
        /// <param name="score">The score vector.</param>
        /// <returns>The fitness.</returns>
        public static double SingleObjectiveMaximise(double[] score)
        {
            //Optimisers should minimise by default!
            return -score[0];
        }
    }
}