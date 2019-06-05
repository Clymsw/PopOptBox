using System;
using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Management;

namespace Optimisation.Base.Helpers
{
    public static class ConvergenceCheckers
    {
        /// <summary>
        ///     Checks to see if the fitness of the best and worst individuals in a population
        ///     are closer than a specified amount
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">the absolute tolerance in the units of the fitness</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool CheckAbsoluteFitnessDivergence(
            this Population pop,
            double tolerance = 1)
        {
            // Math.Abs is not, strictly speaking, necessary. But this is safe.
            var difference = Math.Abs(pop.Worst().Fitness - pop.Best().Fitness);
            return difference < tolerance;
        }

        /// <summary>
        ///     Checks to see if the fitness of the best and worst individuals in a population
        ///     differ by less than a specified amount, as a fraction of the average
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">relative amount</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool CheckRelativeFitnessDivergence<TDecVec>(
            this Population pop,
            double tolerance = 0.001)
        {
            var difference = pop.Worst().Fitness - pop.Best().Fitness;
            if (Math.Abs(difference) <= double.Epsilon)
                return true;

            var allFitness = pop.GetMemberFitnesses();
            var average = allFitness.Average();
            return Math.Abs(difference / average) < tolerance;
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than a specified amount
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">absolute tolerance, in the units of the decision vector</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool CheckAbsoluteDecVecDivergence(
            this Population pop,
            double tolerance)
        {
            var worst = (IEnumerable<double>)pop.Worst().DecisionVector.Vector;
            var best = (IEnumerable<double>)pop.Best().DecisionVector.Vector;

            var differences = best.Zip(worst, (b, w) => Math.Abs(w - b));
            return differences.All(dif => dif < tolerance);
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than an amount, specified for each element
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">
        ///     a vector of tolerances, the same length as the decision vector.
        ///     Any elements with 0 tolerance are ignored
        /// </param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool CheckAbsoluteDecVecDivergence(
            this Population pop,
            double[] tolerance)
        {
            var worst = (IEnumerable<double>)pop.Worst().DecisionVector.Vector;
            var best = (IEnumerable<double>)pop.Best().DecisionVector.Vector;

            if (best.Count() != tolerance.Length)
                throw new ArgumentOutOfRangeException(nameof(tolerance),
                    "Tolerance must have same length as decision vector");

            var differences = best.Zip(worst, (b, w) => Math.Abs(w - b));
            return AllWithinTolerances(differences, tolerance);
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than a specified amount,
        ///     as a proportion of the best individual's DV
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">relative amount</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool CheckRelativeDecVecDivergence(
            this Population pop,
            double tolerance = 0.001)
        {
            var worstVec = (IEnumerable<double>)pop.Worst().DecisionVector.Vector;
            var bestVec = (IEnumerable<double>)pop.Best().DecisionVector.Vector;

            var relativeDifferences = bestVec.Zip(worstVec, (best, worst) => Math.Abs((worst - best) / best));
            return relativeDifferences.All(dif => dif < tolerance);
        }

        /// <summary>
        ///     Checks to see if each element of the decision vectors of the best and worst
        ///     individuals in a population differ by less than an amount,
        ///     specified as a proportion of the best individual's DV, for each element
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">
        ///     a vector of tolerances, the same length as the decision vector.
        ///     Any elements with 0 tolerance are ignored
        /// </param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool CheckRelativeDecVecDivergence(
            this Population pop,
            double[] tolerance)
        {
            var worstVec = (IEnumerable<double>)pop.Worst().DecisionVector.Vector;
            var bestVec = (IEnumerable<double>)pop.Best().DecisionVector.Vector;

            if (bestVec.Count() != tolerance.Length)
                throw new ArgumentOutOfRangeException(nameof(tolerance),
                    "Tolerance must have same length as decision vector");

            var relativeDifferences = bestVec.Zip(worstVec, (best, worst) => Math.Abs((worst - best) / best));
            return AllWithinTolerances(relativeDifferences, tolerance);
        }

        private static bool AllWithinTolerances(IEnumerable<double> differences, IEnumerable<double> tolerance)
        {
            bool IsWithinTolerance(double dif, double tol)
            {
                return tol <= 0 || dif < tol;
            }

            var withinTolerance = differences.Zip(tolerance, IsWithinTolerance);
            return withinTolerance.All(b => b);
        }
    }
}