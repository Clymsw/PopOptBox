using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Optimisation.Base.Management;
using Optimisation.Base.Runtime;
using Optimisation.Base.Variables;

namespace Optimisation.Problems.HyperparameterOptimisation
{
    /// <summary>
    /// Captures information on the performance of an optimisation algorithm on a given test problem.
    /// Immutable by design.
    /// </summary>
    public sealed class ProblemPerformance
    {
        // Definitions
        private readonly string optimiserDescription; 
        private readonly DecisionVector hyperParameters;
        private readonly IProblemEvaluator problem;

        private double[] globalOptimumSolution;

        // Results
        private readonly DateTime startRunTime;
        private readonly Individual bestFound;
        private readonly List<Individual> allEvaluated;

        #region Constructor

        /// <summary>
        /// Constructs the performance assessment class.
        /// </summary>
        /// <param name="optimiserDescription">A user-friendly description of the optimiser (perhaps its class name).</param>
        /// <param name="hyperParameters">The hyperparameters used for the optimiser.</param>
        /// <param name="problem">The problem description.</param>
        /// <param name="results">The completed optimisation runner.</param>
        public ProblemPerformance(
            string optimiserDescription, 
            DecisionVector hyperParameters,
            IProblemEvaluator problem,
            OptimiserRunner results)
        {
            this.optimiserDescription = optimiserDescription;
            this.hyperParameters = hyperParameters;
            this.problem = problem;

            startRunTime = results.StartTime;
            bestFound = results.BestFound;
            allEvaluated = results.AllEvaluated;
        }

        #endregion

        #region Properties

        public DecisionVector GlobalOptimumLocation => problem.GetGlobalOptimum();

        public DecisionVector BestLocation => bestFound.DecisionVector;

        public double[] GlobalOptimumSolution => calculateGlobalOptimumSolution();

        public double[] BestSolution => bestFound.SolutionVector;
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

        private double[] calculateGlobalOptimumSolution()
        {
            if (globalOptimumSolution == null)
            {
                globalOptimumSolution = problem.Evaluate(GlobalOptimumLocation.Vector.Select(d => (double)d).ToArray());
            }
            return globalOptimumSolution;
        }
    }
}