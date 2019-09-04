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
    /// or for multi-objective optimisation.
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
        
        #region MultiObjective

        /// <summary>
        /// Implements the Crowding Distance algorithm proposed by Deb et al (2002).
        /// Note that the individuals provided should be on the same Pareto Front.
        /// Calculates the crowding distance and assigns it into the individual based on the property name provided.
        /// </summary>
        /// <param name="individuals">Individuals forming a Pareto Front.</param>
        public static void AssignCrowdingDistance(IEnumerable<Individual> individuals, string propertyName)
        {
            var inds = individuals as Individual[] ?? individuals.ToArray();
            for (var m = 0; m < inds[0].SolutionVector.Length; m++)
            {
                var tempSorted = inds.OrderBy(a => a.SolutionVector.ElementAt(m)).ToArray();
                var fmin = tempSorted.First().SolutionVector[m];
                tempSorted.First().SetProperty(propertyName, double.MaxValue);
                var fmax = tempSorted.Last().SolutionVector[m];
                tempSorted.Last().SetProperty(propertyName, double.MaxValue);
                
                for (var i = 1; i < inds.Length - 1; i++)
                {
                    var distance = 0.0;
                    if (m > 0)
                        distance = inds[i].GetProperty<double>(propertyName);
                    
                    if (distance == double.MaxValue)
                        continue;

                    tempSorted.ElementAt(i).SetProperty(propertyName,
                        distance + 
                            (tempSorted.ElementAt(i + 1).SolutionVector.ElementAt(m) -
                             tempSorted.ElementAt(i - 1).SolutionVector.ElementAt(m)) /
                            (fmax - fmin));
                }
            }
        }

        /// <summary>
        /// Gets the individuals on the specified Pareto Front.
        /// </summary>
        /// <param name="population">The population extended by this method.</param>
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
        /// Gets all non-dominated individuals.
        /// </summary>
        /// <remarks>
        /// Should be equivalent to population.ParetoFront(1),
        /// but depends on different property (from <see cref="OptimiserPropertyNames"/>).
        /// </remarks>
        /// <param name="population">The population extended by this method.</param>
        /// <returns>An array of <see cref="Individual"/>s.</returns>
        public static Individual[] NonDominatedSet(this Population population)
        {
            return population.Best().SolutionVector.Length > 1
                ? population
                    .Where(i => i.NonDominationRank() == 0).ToArray()
                : new[] { population.Best() };
        }

        /// <summary>
        /// Gets the non-domination rank of an individual (the number of individuals dominating it).
        /// </summary>
        /// <param name="individual">individuals</param>
        /// <returns>The rank.</returns>
        public static int NonDominationRank(this Individual individual)
        {
            return individual.GetProperty<List<Individual>>(OptimiserPropertyNames.DominatedBy).Count;
        }
        
        /// <summary>
        /// Gets whether an <see cref="Individual"/> dominates another one.
        /// This is defined as:
        ///  - for all objectives, the solution values are equal or worse, and
        ///  - for at least one objective, the solution value is worse. 
        /// </summary>
        /// <param name="individual">The individual extended by this method.</param>
        /// <param name="other">The individual to compare to.</param>
        /// <returns><see langword="true"/> if the other individual is dominated.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the two Solution Vectors have different lengths.</exception>
        public static bool IsDominating(this Individual individual, Individual other)
        {
            if (other.SolutionVector.Length != individual.SolutionVector.Length)
                throw new InvalidOperationException(
                    "Other individual must have the same number of objectives in its Solution Vector.");

            if (individual.SolutionVector.Select((v, i) => v < other.SolutionVector.ElementAt(i)).Any(b => b))
                if (individual.SolutionVector.Select((v, i) => v <= other.SolutionVector.ElementAt(i)).All(b => b))
                    return true;

            return false;
        }
        
        #endregion

        #region Convergence
        
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
            var dvRange = pop.DecisionVectorRangeByFitness().ToArray();

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
        
        #endregion
    }
}