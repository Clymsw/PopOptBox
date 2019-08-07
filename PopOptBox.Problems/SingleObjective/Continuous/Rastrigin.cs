using System;
using System.Collections.Generic;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.SingleObjective.Continuous
{
    public class Rastrigin : ProblemSingleObjectiveContinuous
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for the Generalised Rastrigin Function.
        /// Assumes unconstrained, even though normally checked on a +/-5.12 basis
        /// Global optimum is at [0,0,0,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Rastrigin(int numDims) : base(
            "Generalised Rastrigin Function", 
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, 
                    double.MinValue, double.MaxValue,
                    -5.12, 5.12),
                new double[numDims]))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(DecisionVector location)
        {
            // http://profesores.elo.utfsm.cl/~tarredondo/info/soft-comp/functions/node6.html
            // http://benchmarkfcns.xyz/benchmarkfcns/rastriginfcn.html
            // http://www.sfu.ca/~ssurjano/rastr.html

            double result = 10 * location.Vector.Count;
            for (int i = 0; i < location.Vector.Count; i++)
            {
                result += Math.Pow(Convert.ToDouble(location.Vector[i]), 2) - (10 * Math.Cos(2 * Math.PI * Convert.ToDouble(location.Vector[i])));
            }
            return new[] { result };
        }

        #endregion
    }
}
