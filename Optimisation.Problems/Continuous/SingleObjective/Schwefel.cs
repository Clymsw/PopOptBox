using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Problems.Continuous.SingleObjective
{
    public class Schwefel : ProblemEvaluatorSingleObjective
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for the Schwefel Function.
        /// Constrained on [-500, 500]
        /// Global optimum is at [420.9687, 420.9687, 420.9687,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Schwefel(int numDims) : base(
            "Schwefel Function",
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, -500, 500),
                Enumerable.Repeat(420.9687, numDims).ToArray()))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(double[] location)
        {
            // http://www.sfu.ca/~ssurjano/schwef.html
            // http://benchmarkfcns.xyz/benchmarkfcns/schwefelfcn.html

            double result = 418.9829 * Convert.ToDouble(location.Length);
            for (int i = 0; i < location.Length; i++)
            {
                result -= location[i] *
                    Math.Sin(Math.Sqrt(Math.Abs(location[i])));
            }
            return new[] { result };
        }

        #endregion
    }
}
