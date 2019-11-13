using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective.Continuous
{
    public class Zdt6 : ProblemMultipleObjective
    {
        /// <summary>
        /// Creates an evaluator for the ZDT6 problem.
        /// 10D Decision space constrained on [0,1]
        /// Two objectives, with Pareto optimal values at [1-exp(-4.x1).(sin(6.pi.x1)^6), exp(-8.x1).(sin(6.pi.x1)^12)-2.exp(-4.x1).(sin(6.pi.x1)^6)]
        /// </summary>
        public Zdt6() : base(
            "ZDT6", 
            DecisionSpace.CreateForUniformDoubleArray(10, 0,1,0,1),
            ContinuousProblemPropertyNames.TheLocation,
            ContinuousProblemPropertyNames.Result + "1", ContinuousProblemPropertyNames.Result + "2")
        {
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            // Zitzler, Deb, Thiele, "Comparison of Multiobjective Evolutionary Algorithms: Empirical Results", 2000 
            var numDims = definition.Count;

            var x1 = (double)definition.ElementAt(0);
            var f1 = 1 - Math.Exp(-4 * x1) * Math.Pow(Math.Sin(6 * Math.PI * x1), 6.0); 
                
            var g = 0.0;
            for (var i = 1; i < numDims; i++)
            {
                g += (double) definition.ElementAt(i) / (numDims - 1);
            }
            g = 1 + 9 * Math.Pow(g, 0.25);

            var h = 1 - Math.Pow(f1 / g, 2.0);

            var f2 = g * h;

            return new[] {f1, f2};
        }

        public override DecisionVector[] GetOptimalParetoFront(int numberOfPoints)
        {
            var xm = Enumerable.Repeat(0.0, 29);
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