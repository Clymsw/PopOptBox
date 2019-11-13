using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective.Continuous
{
    public class Dtlz1 : ProblemMultipleObjective
    {
        private readonly int numberOfDimensions;
        private readonly int numberOfObjectives;
        
        /// <summary>
        /// Creates an evaluator for the DTLZ1 problem.
        /// N-D Decision space constrained on [0,1]
        /// M minimisation objectives, with Pareto optimal values at X_M = 0
        /// </summary>
        /// <param name="numberOfInputDimensions">The number of dimensions for the decision space.</param>
        /// <param name="numberOfObjectives">The number of objectives.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the number of input dimensions is less than the number of objectives.</exception>
        public Dtlz1(int numberOfInputDimensions, int numberOfObjectives = 5)
            : base("DTLZ1",
                DecisionSpace.CreateForUniformDoubleArray(numberOfInputDimensions, 0, 1, 0, 1),
                ContinuousProblemPropertyNames.TheLocation,
                Enumerable.Range(1, numberOfObjectives)
                    .Select(m => ContinuousProblemPropertyNames.Result + m.ToString()).ToArray())
        {
            if (numberOfInputDimensions < numberOfObjectives)
                throw new ArgumentOutOfRangeException(nameof(numberOfInputDimensions),
                    "The number of input dimensions must be at least as large as the number of objectives.");
            
            numberOfDimensions = numberOfInputDimensions;
            this.numberOfObjectives = numberOfObjectives;
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            //Deb, Thiele, Laumanns and Zitzler "Scalable Test Problems for Evolutionary Multi-Objective Optimisation" 2001
            var inputs = definition.Select(x => Convert.ToDouble(x)).ToArray();
            var g = 100 * (numberOfObjectives + inputs
                               .Take(numberOfObjectives)
                               .Select(x => Math.Pow(x - 0.5, 2.0) - Math.Cos(20 * Math.PI * (x - 0.5)))
                               .Sum());
            var objectives = new double[numberOfObjectives];
            for (var j = 0; j < numberOfObjectives - 1; j++)
            {
                if (j == 0)
                {
                    objectives[numberOfObjectives - 1] = 0.5 * (1 + g) * (1 - inputs.ElementAt(0));
                }
                else
                {
                    objectives[numberOfObjectives - j - 1] = objectives[numberOfObjectives - j] 
                                    / (1 - inputs.ElementAt(j - 1)) 
                                    * inputs.ElementAt(j - 1) 
                                    * (1 - inputs.ElementAt(j));
                }
                objectives[0] = objectives[1]
                                / (1 - inputs.ElementAt(numberOfObjectives - 2))
                                * inputs.ElementAt(numberOfObjectives - 2);
            }
            return objectives;
        }

        public override DecisionVector[] GetOptimalParetoFront(int numberOfPoints)
        {
            var pf = new List<DecisionVector>();
            
            var rng = new MersenneTwister();
            var numPerDim = (double) numberOfPoints / (numberOfDimensions - numberOfObjectives);
            var dim = numberOfObjectives;
            var loops = 0;
            for (var i = 0; i < numberOfPoints; i++)
            {
                // All Pareto-optimal values of the M dimensions are zero
                var xn = new double[numberOfDimensions];

                for (var d = numberOfObjectives; d < numberOfDimensions; d++)
                {
                    if (d == dim && loops < numPerDim)
                    {
                        if (numPerDim < 1)
                            xn[d] = 0.0;
                        else
                            xn[d] = loops / Math.Floor(numPerDim);
                    }
                    else
                    {
                        xn[d] = rng.NextDouble();
                    }
                }

                pf.Add(DecisionVector.CreateFromArray(
                    DecisionSpace.CreateForUniformDoubleArray(numberOfDimensions, 0, 1, 0, 1),
                    xn));
                
                dim++;
                if (dim == numberOfDimensions)
                {
                    dim = numberOfObjectives;
                    loops++;
                }
            }

            return pf.ToArray();
        }
    }
}