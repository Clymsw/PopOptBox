using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Problems.Continuous.SingleObjective
{
    public class Rosenbrock : ProblemEvaluatorContinuousSingleObjective
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for the Rosenbrock Function.
        /// Assumes unconstrained, even though normally checked on a [-5, 10] basis.
        /// Global optimum is at [1,1,1,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Rosenbrock(int numDims) : base(
            "Rosenbrock Function", 
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims,
                    double.MinValue, double.MaxValue,
                    -5.0, 10.0),
                Enumerable.Repeat(1.0, numDims).ToArray()))
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
                result += 100 * Math.Pow(location[i + 1] - Math.Pow(location[i], 2), 2)
                          + Math.Pow(1 - location[i], 2);
            }
            return new[] { result };
        }

        #endregion
    }
}
