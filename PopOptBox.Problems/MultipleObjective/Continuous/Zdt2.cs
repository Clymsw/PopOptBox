using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective.Continuous
{
    public class Zdt2 : ProblemMultipleObjective
    {
        private readonly int numberOfDimensions;
        
        /// <summary>
        /// Creates an evaluator for the ZDT2 problem.
        /// N-D Decision space constrained on [0,1]
        /// Two minimisation objectives, with Pareto optimal values at [x1, 1 - x1^2]
        /// </summary>
        /// <param name="numberOfDimensions">The number of input dimensions (default is 30)</param>
        public Zdt2(int numberOfDimensions = 30) : base(
            "ZDT2", 
            DecisionSpace.CreateForUniformDoubleArray(30, 0,1,0,1),
            ContinuousProblemPropertyNames.TheLocation,
            ContinuousProblemPropertyNames.Result1, ContinuousProblemPropertyNames.Result2)
        {
            this.numberOfDimensions = numberOfDimensions;
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            // Zitzler, Deb, Thiele, "Comparison of Multiobjective Evolutionary Algorithms: Empirical Results", 2000 
            var numDims = definition.Count;

            var f1 = (double)definition.ElementAt(0);

            var g = 0.0;
            for (var i = 1; i < numDims; i++)
            {
                g += (double) definition.ElementAt(i) / (numDims - 1);
            }
            g *= 9;
            g += 1;

            var h = 1 - Math.Pow(f1 / g, 2.0);

            var f2 = g * h;

            return new[] {f1, f2};
        }

        public override DecisionVector[] GetOptimalParetoFront(int numberOfPoints)
        {
            var xm = Enumerable.Repeat(0.0, numberOfDimensions - 1);
            var x1 = Enumerable.Range(0, numberOfPoints).Select(
                i => new List<double> {(double) i / numberOfPoints});
            
            var pf = new List<DecisionVector>();
            foreach (var f1 in x1)
            {
                f1.AddRange(xm.ToArray());
                pf.Add(DecisionVector.CreateFromArray(decisionSpace,
                    f1));
            }

            return pf.ToArray();
        }
    }
}