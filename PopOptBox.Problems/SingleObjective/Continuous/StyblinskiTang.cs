using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.SingleObjective.Continuous
{
    public class StyblinskiTang : ProblemSingleObjectiveContinuous
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
                DecisionSpace.CreateForUniformDoubleArray(numDims, 
                    -5, 5,
                    -4.5, 2.5),
                Enumerable.Repeat(-2.903534, numDims).ToArray()))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(DecisionVector location)
        {
            // http://www.sfu.ca/~ssurjano/stybtang.html
            // http://benchmarkfcns.xyz/benchmarkfcns/styblinskitankfcn.html

            double result = 0;
            for (int i = 0; i < location.Count; i++)
            {
                result += Math.Pow(Convert.ToDouble(location[i]), 4) -
                    16 * Math.Pow(Convert.ToDouble(location[i]), 2) +
                    5 * Convert.ToDouble(location[i]);
            }
            return new[] { result / 2 };
        }

        #endregion
    }
}
