using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective.Continuous
{
    public class Zdt4 : ProblemMultipleObjective
    {
        /// <summary>
        /// Creates an evaluator for the ZDT4 problem.
        /// 10D Decision space constrained on [0,1] for x1 and [-5,5] for the rest.
        /// Two objectives, with Pareto optimal values at [x1, 1 - sqrt(x1)]
        /// </summary>
        public Zdt4() : base(
            "ZDT4", 
            new DecisionSpace(Enumerable.Range(0,10).Select(
                i => i == 0 
                    ? new VariableContinuous(0,1,0,1)
                    : new VariableContinuous(-5,5,-5,5))),
            ContinuousProblemPropertyNames.TheLocation,
            ContinuousProblemPropertyNames.Result + "1", ContinuousProblemPropertyNames.Result + "2")
        {
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            // Zitzler, Deb, Thiele, "Comparison of Multiobjective Evolutionary Algorithms: Empirical Results", 2000 
            var numDims = definition.Count;

            var f1 = (double)definition.ElementAt(0);

            var g = 1.0 + 10.0 * (numDims - 1.0);
            for (var i = 1; i < numDims; i++)
            {
                var xi = (double) definition.ElementAt(i);
                g += Math.Pow(xi, 2.0) - 10 * Math.Cos(4 * Math.PI * xi);
            }

            var h = 1 - Math.Sqrt(f1 / g);

            var f2 = g * h;

            return new[] {f1, f2};
        }

        public override DecisionVector[] GetOptimalParetoFront(int numberOfPoints)
        {
            throw new NotImplementedException();
        }
    }
}