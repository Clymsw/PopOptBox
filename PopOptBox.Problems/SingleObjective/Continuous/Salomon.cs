using System;
using System.Collections.Generic;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.SingleObjective.Continuous
{
    public class Salomon : ProblemSingleObjectiveContinuous
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for Salomon's Function.
        /// Assumes unconstrained, even though normally checked on a +/-100 basis
        /// Global optimum is at [0,0,0,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Salomon(int numDims) : base(
            "Salomon's Function",
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, 
                    double.MinValue, double.MaxValue,
                    -100, 100),
                new double[numDims]))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(DecisionVector location)
        {
            // http://profesores.elo.utfsm.cl/~tarredondo/info/soft-comp/functions/node12.html
            // http://benchmarkfcns.xyz/benchmarkfcns/salomonfcn.html

            double temp = 0;
            for (var i = 0; i < location.Vector.Count; i++)
            {
                temp += Math.Pow(Convert.ToDouble(location.Vector[i]), 2);
            }
            return new[] { 1 - Math.Cos(2 * Math.PI * Math.Sqrt(temp)) + 0.1 * Math.Sqrt(temp) };
        }

        #endregion
    }
}
