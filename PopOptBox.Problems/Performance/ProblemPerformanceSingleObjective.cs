using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PopOptBox.Base;
using PopOptBox.Base.Management;
using PopOptBox.Base.Runtime;
using PopOptBox.Base.Variables;
using PopOptBox.Problems.SingleObjective;

namespace PopOptBox.Problems.Performance
{
    /// <summary>
    /// Captures information on the performance of an optimisation algorithm on a given test problem.
    /// Immutable by design.
    /// </summary>
    public sealed class ProblemPerformanceSingleObjective
    {
        // Definitions
        private readonly string optimiserDescription;
        private readonly ProblemSingleObjective problem;

        public readonly double[] GlobalOptimumSolution;

        // Results
        private readonly DateTime startRunTime;
        private readonly Individual bestFound;
        private readonly List<Individual> allEvaluated;

        #region Constructor

        /// <summary>
        /// Constructs the performance assessment class.
        /// </summary>
        /// <param name="optimiserDescription">A user-friendly description of the optimiser (perhaps its class name).</param>
        /// <param name="problem">The problem description.</param>
        /// <param name="results">The completed optimisation runner.</param>
        public ProblemPerformanceSingleObjective(
            string optimiserDescription,
            ProblemSingleObjective problem,
            OptimiserRunner results)
        {
            this.optimiserDescription = optimiserDescription;
            this.problem = problem;

            if (results.StartTime == null || results.BestFound == null || results.AllEvaluated == null)
                throw new ArgumentNullException(nameof(results), "Optimisation has not been run.");

            startRunTime = (DateTime)results.StartTime;
            bestFound = results.BestFound;
            allEvaluated = results.AllEvaluated;

            GlobalOptimumSolution = problem.Evaluate(problem.GetGlobalOptimum()).ToArray();
        }

        #endregion

        #region Properties

        public DecisionVector GlobalOptimumLocation => problem.GetGlobalOptimum();

        public DecisionVector BestLocation => bestFound.DecisionVector;

        

        public double[]? BestSolution => bestFound.SolutionVector;
        public double BestFitness => bestFound.Fitness;

        public TimeSpan TimeToFindBest => bestFound
            .GetProperty<DateTime>(OptimiserPropertyNames.CreationTime)
            - startRunTime;

        public int EvaluationsToFindBest => bestFound
            .GetProperty<int>(OptimiserPropertyNames.CreationIndex);

        public TimeSpan TimeToConverge => allEvaluated
            .Select(i => i.GetProperty<DateTime>(OptimiserPropertyNames.ReinsertionTime))
            .OrderBy(t => t)
            .Last()
            - startRunTime;

        public double EvaluationsToConverge => allEvaluated
            .Select(i => i.GetProperty<int>(OptimiserPropertyNames.ReinsertionIndex))
            .OrderBy(t => t)
            .Last();

        #endregion

        public override string ToString()
        {
            return $"Testing {optimiserDescription} on {problem}, reached {string.Join(" - ", bestFound.SolutionVector.Select(d => d.ToString("F3", NumberFormatInfo.InvariantInfo)))}.";
        }
    }
}