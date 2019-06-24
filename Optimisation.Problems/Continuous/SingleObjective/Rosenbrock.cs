﻿using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;

namespace Optimisation.Problems.Continuous.SingleObjective
{
    public class Rosenbrock : ProblemEvaluatorSingleObjective
    {
        #region Contructor
        /// <summary>
        /// Creates an evaluator for the Rosenbrock Function.
        /// Constrained on [-5, 10]
        /// Global optimum is at [0,0,0,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Rosenbrock(int numDims) : base(
            "Rosenbrock Function", 
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, double.MinValue, double.MaxValue),
                new double[numDims]))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(double[] location)
        {
            // http://www.sfu.ca/~ssurjano/rosen.html
            // http://benchmarkfcns.xyz/benchmarkfcns/rosenbrockfcn.html

            double result = 0;
            for (int i = 0; i < location.Length - 1; i++)
            {
                result += 100 * Math.Pow(location[i + 1] - Math.Pow(location[i], 2), 2) +
                    Math.Pow(1 - location[i], 2);
            }
            return new[] { result };
        }

        #endregion
    }
}
