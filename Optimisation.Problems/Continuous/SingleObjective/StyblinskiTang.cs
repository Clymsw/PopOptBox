using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Problems.Continuous.SingleObjective
{
    public class StyblinskiTang : ProblemEvaluatorSingleObjective
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for the Styblinski-Tang Function.
        /// Constrained on [-5, 5]
        /// Global optimum is at [-2.903534, -2.903534, -2.903534,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public StyblinskiTang(int numDims) : base(
            "Styblinski-Tang Function",
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, -5, 5),
                Enumerable.Repeat(-2.903534, numDims).ToArray()))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(double[] location)
        {
            // http://www.sfu.ca/~ssurjano/stybtang.html
            // http://benchmarkfcns.xyz/benchmarkfcns/styblinskitankfcn.html

            double result = 0;
            for (int i = 0; i < location.Length; i++)
            {
                result += Math.Pow(location[i], 4) -
                    16 * Math.Pow(location[i], 2) +
                    5 * location[i];
            }
            return new[] { result / 2 };
        }

        #endregion
    }
}
