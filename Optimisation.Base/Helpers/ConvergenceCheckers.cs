using System;
using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Management;

namespace Optimisation.Base.Helpers
{
    /// <summary>
    /// Helper functions to see (in different ways) if an optimisation has converged,
    /// <see cref="Runtime.OptimiserRunnerBasic"/> and <see cref="Runtime.OptimiserRunnerParallel"/>.
    /// </summary>
    public static class ConvergenceCheckers
    {
        /// <summary>
        ///     Checks to see if the fitness of the best and worst individuals in a population
        ///     are closer than a specified amount.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">the absolute tolerance in the units of the fitness</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool AbsoluteFitnessConvergence(
            this Population pop,
            double tolerance = 1)
        {
            return pop.FitnessRange() <= tolerance;
        }

        /// <summary>
        ///     Checks to see if the fitness of the best and worst individuals in a population
        ///     differ by less than a specified amount, as a fraction of the average.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">relative amount</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool RelativeFitnessConvergence(
            this Population pop,
            double tolerance = 0.001)
        {
            if (Math.Abs(pop.FitnessRange()) <= double.Epsilon)
                return true;

            return Math.Abs(pop.FitnessRange() / pop.FitnessAverage()) <= tolerance;
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than a specified amount.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">absolute tolerance, in the units of the decision vector</param>
        /// <returns><see langword="true" /> if converged</returns>
        /// <exception cref="ArgumentException">If the individuals do not have the same decision space.</exception>
        public static bool AbsoluteDecisionVectorConvergence(
            this Population pop,
            double tolerance)
        {
            var dvRange = pop.DecisionVectorRangeByFitness();
            return dvRange.All(dif => dif <= tolerance);
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than an amount, specified for each element.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">
        ///     a vector of tolerances, the same length as the decision vector.
        ///     Any elements with 0 tolerance are ignored.
        /// </param>
        /// <returns><see langword="true" /> if converged</returns>
        /// <exception cref="ArgumentException">If the individuals do not have the same decision space, or if the tolerance does not have the right length.</exception>
        public static bool AbsoluteDecisionVectorConvergence(
            this Population pop,
            double[] tolerance)
        {
            var dvRange = pop.DecisionVectorRangeByFitness();

            if (dvRange.Count() != tolerance.Length)
                throw new ArgumentException("Tolerance must have same length as decision vector",
                    nameof(tolerance));

            return AllWithinTolerances(dvRange, tolerance);
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than a specified amount,
        ///     as a proportion of the best individual's DV.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">relative amount</param>
        /// <returns><see langword="true" /> if converged</returns>
        /// <exception cref="ArgumentException">If the individuals do not have the same decision space.</exception>
        public static bool RelativeDecisionVectorDivergence(
            this Population pop,
            double tolerance = 0.001)
        {
            var dvRange = pop.DecisionVectorRangeByFitness();
            var relativeDifferences = pop.Best().DecisionVector.Vector
                .Select((n,i) => dvRange.ElementAt(i) / (double) n);
            
            return relativeDifferences.All(r => r <= tolerance);
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than an amount,
        ///     specified as a proportion of the best individual's DV, for each element.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">
        ///     a vector of tolerances, the same length as the decision vector.
        ///     Any elements with 0 tolerance are ignored
        /// </param>
        /// <returns><see langword="true" /> if converged</returns>
        /// <exception cref="ArgumentException">If the individuals do not have the same decision space, or if the tolerance does not have the right length.</exception>
        public static bool RelativeDecisionVectorDivergence(
            this Population pop,
            double[] tolerance)
        {
            var dvRange = pop.DecisionVectorRangeByFitness();

            if (dvRange.Count() != tolerance.Length)
                throw new ArgumentException("Tolerance must have same length as decision vector",
                    nameof(tolerance));
            
            var relativeDifferences = pop.Best().DecisionVector.Vector
                .Select((n,i) => dvRange.ElementAt(i) / (double) n);

            return AllWithinTolerances(relativeDifferences, tolerance);
        }

        private static bool AllWithinTolerances(IEnumerable<double> differences, IEnumerable<double> tolerance)
        {
            bool IsWithinTolerance(double dif, double tol)
            {
                return tol <= 0 || dif <= tol;
            }

            var withinTolerance = differences.Zip(tolerance, IsWithinTolerance);
            return withinTolerance.All(b => b);
        }
    }
}