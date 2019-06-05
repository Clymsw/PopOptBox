using System;
using System.Collections.Generic;
using Optimisation.Base.Management;
using Optimisation.Base.Conversion;

namespace Optimisation.Base.Runtime
{
    public sealed class OptimiserRunnerBasic : OptimiserRunner
    {
        private readonly OptimiserBuilder builder;
        private readonly IEvaluator evaluator;
        private readonly Func<Population, bool> convergenceCheckers;
        private readonly Action<KeyValuePair<int, Population>> reporters;

        private volatile bool cancelDemanded;

        public OptimiserRunnerBasic(
            OptimiserBuilder builder,
            IEvaluator evaluator,
            Func<Population, bool> convergenceCheckers,
            Action<KeyValuePair<int, Population>> reporters)
        {
            this.builder = builder;
            this.evaluator = evaluator;
            this.convergenceCheckers = convergenceCheckers;
            this.reporters = reporters;
        }

        public override void Cancel()
        {
            cancelDemanded = true;
        }

        public override void Run(
            bool storeAll = true,
            int timeOut = 0,
            int reportingFrequency = 100)
        {
            // Initialise
            var optimiser = builder.CreateOptimiser();
            var model = builder.CreateModel();
            cancelDemanded = false;

            //Setup
            var nextInds = optimiser.GetNextToEvaluate(1);
            var nextInd = nextInds[0];

            if (timeOut == 0)
            {
                var numDims = nextInd.DecisionVector.Vector.Count;
                timeOut = Math.Min(numDims * 20000, 2000000);
            }

            AllEvaluated = new List<KeyValuePair<int, Individual>>();
            FinalPopulation = null;
            BestFound = new KeyValuePair<int, Individual>(0, nextInd);

            var generationNumber = 0;

            //Go!
            while (nextInd.DecisionVector.Vector.Count > 0)
            {
                nextInd.SetProperty(
                    OptimiserDefinitions.CreationIndex,
                    generationNumber);

                // Evaluate
                model.PrepareForEvaluation(nextInd);
                evaluator.Evaluate(nextInd);

                // Reinsert
                var returnInds = new List<Individual>
                {
                    nextInd
                };
                optimiser.ReInsert(returnInds);

                nextInd.SetProperty(
                    OptimiserDefinitions.ReinsertionIndex,
                    generationNumber);

                // Store
                if (storeAll)
                {
                    AllEvaluated.Add(new KeyValuePair<int, Individual>(generationNumber, nextInd));
                }

                // Update best
                var bestInd = optimiser.Population.Best();
                if (bestInd != null && bestInd.Fitness < BestFound.Value.Fitness)
                    BestFound = new KeyValuePair<int, Individual>(generationNumber, bestInd);

                // Create individuals for next loop
                generationNumber++;

                nextInds = optimiser.GetNextToEvaluate(1);
                nextInd = nextInds[0];

                // Check for completion
                if (generationNumber >= timeOut || cancelDemanded)
                {
                    //Bored...
                    break;
                }
                if (optimiser.Population.IsTargetSizeReached)
                {
                    if (convergenceCheckers(optimiser.Population))
                        break;
                }
                if (generationNumber % reportingFrequency == 0)
                {
                    reporters(new KeyValuePair<int, Population>(
                        generationNumber, optimiser.Population));
                }
            }

            reporters(new KeyValuePair<int, Population>(
                generationNumber, optimiser.Population));

            //Finish off
            FinalPopulation = optimiser.Population;
        }
    }
}