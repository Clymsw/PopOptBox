﻿using Optimisation.Base.Variables;
using System;

namespace Optimisation.Problems.Continuous.SingleObjective
{
    public class Ellipsoidal : ProblemEvaluatorSingleObjective
    {
        #region Contructor
        /// <summary>
        /// Creates an evaluator for the Ellipsoidal Function.
        /// Constrained on [-10, 10]
        /// Global optimum is at [0,0,0,...]
        /// </summary>
        /// <param name="numDims">Number of input dimensions</param>
        public Ellipsoidal(int numDims) : base(
            "Ellipsoidal Function",
            DecisionVector.CreateFromArray(
                DecisionSpace.CreateForUniformDoubleArray(numDims, -10, 10),
                new double[numDims]))
        {
        }

        #endregion

        #region Implement abstract

        protected override double GetFitness(double[] location)
        {
            // http://profesores.elo.utfsm.cl/~tarredondo/info/soft-comp/functions/node3.html

            double result = 0;
            for (int i = 0; i < location.Length - 1; i++)
            {
                result += (i + 1) * Math.Pow(location[i], 2);
            }
            return result;
        }

        #endregion
    }
}