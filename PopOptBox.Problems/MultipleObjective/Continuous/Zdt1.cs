using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective.Continuous
{
    public class Zdt1 : ProblemMultipleObjective
    {
        private readonly int numberOfDimensions;
        
        /// <summary>
        /// Creates an evaluator for the ZDT1 problem.
        /// N-D Decision space constrained on [0,1]
        /// Two minimisation objectives, with Pareto optimal values at [x1, 1 - sqrt(x1)]
        /// </summary>
        /// <param name="numberOfDimensions">The number of input dimensions (default is 30)</param>
        public Zdt1(int numberOfDimensions = 30) : base(
            "ZDT1", 
            DecisionSpace.CreateForUniformDoubleArray(numberOfDimensions, 0, 1+1e12, 0, 1+1e12),
            ContinuousProblemPropertyNames.TheLocation,
            ContinuousProblemPropertyNames.Result + "1", ContinuousProblemPropertyNames.Result + "2")
        {
            this.numberOfDimensions = numberOfDimensions;
        }

        public override IEnumerable<double> Evaluate(DecisionVector definition)
        {
            // Zitzler, Deb, Thiele, "Comparison of Multiobjective Evolutionary Algorithms: Empirical Results", 2000 
            var numDims = definition.Count;

            var f1 = (double)definition.ElementAt(0);

            var g = definition.Sum(d => (double)d) - f1;
            g = 1.0 + (9.0 / (numDims - 1.0) * g);

            var h = 1 - Math.Sqrt(f1 / g);

            var f2 = g * h;

            return new[] {f1, f2};
        }

        public override DecisionVector[] GetOptimalParetoFront(int numberOfPoints)
        {
            var xm = Enumerable.Repeat(0.0, numberOfDimensions - 1);
            var x1 = Enumerable.Range(0, numberOfPoints)
                .Select(i => new List<double> { (double)i / (numberOfPoints - 1) });
            
            var pf = new List<DecisionVector>();
            foreach (var f1 in x1)
            {
                f1.AddRange(xm.ToArray());
                pf.Add(
                    DecisionVector.CreateFromArray(decisionSpace, f1));
            }

            return pf.ToArray();
        }
    }
}