using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;
using MathNet.Numerics.LinearAlgebra;

namespace PopOptBox.Base.Calculation
{
    /// <summary>
    /// Helper functions to calculate <see cref="Population"/> metrics, 
    /// for example to support calculating whether an optimisation has converged,
    /// or for multiobjective optimisation.
    /// </summary>
    public static class PopulationMetrics
    {
        /// <summary>
        /// Gets the centroid of the population (the average location of all the decision vectors).
        /// </summary>
        /// <param name="population">The population.</param>
        /// <returns>A double array representing the centroid location.</returns>
        public static double[] Centroid(this Population population)
        {
            if (!population.ConstantLengthDecisionVector) 
                throw new InvalidOperationException("This function is not valid for a population with variable length Decision Vectors.");
            
            var decisionVectors = population.GetMemberDecisionVectors().Select(dv => dv.Select(d => (double) d));
            var matrix = Matrix<double>.Build.DenseOfColumns(decisionVectors);
            var centroid = matrix.RowSums() / matrix.ColumnCount;
            return centroid.ToArray();
        }
        
        /// <summary>
        /// Gets the individuals on the specified Pareto Front.
        /// </summary>
        /// <param name="front">The desired Pareto Front (1 is the first, by tradition).</param>
        /// <returns>An array of <see cref="Individual"/>s.</returns>
        public static Individual[] ParetoFront(this Population population, int front = 1)
        {
            return population.Best().SolutionVector.Length > 1
                   ? population
                       .Where(i => i.GetProperty<int>(OptimiserPropertyNames.ParetoFront) == front).ToArray()
                   : front < population.Count
                     ? new [] { population[front - 1] }
                     : new Individual[0];
        }

        /// <summary>
        ///     Checks to see if the Fitness of the best and worst individuals in a population
        ///     are closer than a specified amount.
        /// </summary>
        /// <param name="pop">population</param>
        /// <param name="tolerance">the absolute tolerance in the units of the Fitness</param>
        /// <returns><see langword="true" /> if converged</returns>
        public static bool AbsoluteFitnessConvergence(
            this Population pop,
            double tolerance = 1)
        {
            return pop.FitnessRange() <= tolerance;
        }

        /// <summary>
        ///     Checks to see if the Fitness of the best and worst individuals in a population
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
        /// <param name="tolerance">absolute tolerance, in the units of the Decision Vector</param>
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
        ///     a vector of tolerances, the same length as the Decision Vector.
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
                throw new ArgumentException("Tolerance must have same length as Decision Vector",
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
            var relativeDifferences = pop.Best().DecisionVector
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
        ///     a vector of tolerances, the same length as the Decision Vector.
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
                throw new ArgumentException("Tolerance must have same length as Decision Vector",
                    nameof(tolerance));
            
            var relativeDifferences = pop.Best().DecisionVector
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