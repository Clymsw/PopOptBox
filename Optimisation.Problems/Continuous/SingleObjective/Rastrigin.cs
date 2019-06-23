using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;

namespace Optimisation.Problems.Continuous.SingleObjective
{
    public class Rastrigin : ProblemEvaluatorSingleObjective
    {
        #region Contructor
        /// <summary>
        /// Creates an evaluator for the Generalised Rastrigin Function.
        /// Assumes unconstrained, even though normally checked on a +/-5.12 basis
        /// Global optimum is at [0,0,0,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Rastrigin(int numDims) : base(
            "Generalised Rastrigin Function", 
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, double.MinValue, double.MaxValue),
                new double[numDims]))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(double[] location)
        {
            // http://profesores.elo.utfsm.cl/~tarredondo/info/soft-comp/functions/node6.html
            // http://benchmarkfcns.xyz/benchmarkfcns/rastriginfcn.html
            // http://www.sfu.ca/~ssurjano/rastr.html

            double result = 10 * location.Length;
            for (int i = 0; i < location.Length - 1; i++)
            {
                result += Math.Pow(location[i], 2) - (10 * Math.Cos(2 * Math.PI * location[i]));
            }
            return new[] { result };
        }

        #endregion
    }
}
