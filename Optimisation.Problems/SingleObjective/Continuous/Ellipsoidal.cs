using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;

namespace Optimisation.Problems.SingleObjective.Continuous
{
    public class Ellipsoidal : ProblemSingleObjectiveContinuous
    {
        #region Constructor
        /// <summary>
        /// Creates an evaluator for the Ellipsoidal Function.
        /// Constrained on [-10, 10]
        /// Global optimum is at [0,0,0,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Ellipsoidal(int numDims) : base(
            "Ellipsoidal Function",
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, 
                    -10, 10, 
                    -5, 5),
                new double[numDims]))
        {
        }

        #endregion

        #region Implement abstract

        public override IEnumerable<double> Evaluate(DecisionVector location)
        {
            // http://profesores.elo.utfsm.cl/~tarredondo/info/soft-comp/functions/node3.html

            double result = 0;
            for (int i = 0; i < location.Vector.Count; i++)
            {
                result += (i + 1) * Math.Pow(Convert.ToDouble(location.Vector[i]), 2);
            }
            return new[] { result };
        }

        #endregion
    }
}
