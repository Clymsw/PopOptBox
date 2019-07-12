using System;
using System.Collections.Generic;
using System.Linq;
using Optimisation.Base.Variables;

namespace Optimisation.Problems.HyperparameterOptimisation
{
    /// <summary>
    /// Captures information on the performance of an optimisation algorithm on a given test problem.
    /// Immutable by design.
    /// </summary>
    public sealed class OptimiserPerformance
    {
        // Definitions
        private readonly string optimiser; 
        private readonly DecisionVector hyperParameters;
        private readonly string problem;
        
        // Results
        private readonly List<double> fitnessResults;
        private readonly List<TimeSpan> timesToConverge;
        private readonly List<int> evaluationsToConverge;
        private readonly List<int> evaluationsToFindBest;

        #region Constructor

        public OptimiserPerformance(string optimiser, DecisionVector hyperParameters, string problem,
            List<double> fitnessResults, List<TimeSpan> timesToConverge, 
            List<int> evaluationsToConverge,List<int> evaluationsToFindBest)
        {
            this.optimiser = optimiser;
            this.hyperParameters = hyperParameters;
            this.problem = problem;
            this.fitnessResults = fitnessResults;
            this.evaluationsToConverge = evaluationsToConverge;
            this.timesToConverge = timesToConverge;
            this.evaluationsToFindBest = evaluationsToFindBest;
        }

        #endregion 
        
        public double MeanFitness => fitnessResults.Average();

        public double BestFitness => fitnessResults.Min();
        
        public double ProportionOfFitnessValuesBelow(double value)
        {
            return (double)fitnessResults.Count(f => f <= value)
                   / fitnessResults.Count;
        }

        public TimeSpan MeanTimeToConverge => TimeSpan.FromMilliseconds(timesToConverge.Average(t => t.Milliseconds));
        
        public double MeanEvaluationsToConverge => evaluationsToConverge.Average();
        
        public double MeanEvaluationsToFindBest => evaluationsToFindBest.Average();

        public override string ToString()
        {
            return $"Testing {optimiser} {fitnessResults.Count} times on {problem}.";
        }
    }
}