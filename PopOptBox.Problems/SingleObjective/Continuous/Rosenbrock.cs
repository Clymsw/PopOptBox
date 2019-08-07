using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.SingleObjective.Continuous
{
    public class Rosenbrock : ProblemSingleObjectiveContinuous
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

        public override IEnumerable<double> Evaluate(DecisionVector location)
        {
            // http://www.sfu.ca/~ssurjano/rosen.html
            // http://benchmarkfcns.xyz/benchmarkfcns/rosenbrockfcn.html

            double result = 0;
            for (int i = 0; i < location.Vector.Count - 1; i++)
            {
                result += 100 * Math.Pow(Convert.ToDouble(location.Vector[i + 1]) - Math.Pow(Convert.ToDouble(location.Vector[i]), 2), 2)
                          + Math.Pow(1 - Convert.ToDouble(location.Vector[i]), 2);
            }
            return new[] { result };
        }

        #endregion
    }
}
