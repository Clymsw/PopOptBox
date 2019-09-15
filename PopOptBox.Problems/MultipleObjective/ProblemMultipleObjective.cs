using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;

namespace PopOptBox.Problems.MultipleObjective
{
    public abstract class ProblemMultipleObjective : Evaluator<DecisionVector>
    {
        private readonly string name;
        private int numberOfObjectives;
        private readonly List<DecisionVector> optimalParetoFrontMembers;
        
        protected ProblemMultipleObjective(
            string name, IEnumerable<DecisionVector> optimalParetoFrontMembers,
            string definitionKey, params string[] solutionKeys) : base(definitionKey, solutionKeys)
        {
            var optimalParetoFront = optimalParetoFrontMembers.ToList();
            if (optimalParetoFront.First().Count < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(optimalParetoFrontMembers),
                    "Number of dimensions must be greater than zero.");
            }
            this.name = name;
            numberOfObjectives = solutionKeys.Length;
            this.optimalParetoFrontMembers = optimalParetoFront;
        }

        /// <summary>
        /// Gets a number of locations specifying the optimal Pareto Front
        /// </summary>
        /// <returns>An array of <see cref="DecisionVector"/>s.</returns>
        public DecisionVector[] GetOptimalParetoFront()
        {
            return optimalParetoFrontMembers.ToArray();
        }
        
        public override bool GetLegality(DecisionVector definition)
        {
            // By definition, if we can construct a Decision Vector in the same decision space, it is legal
            return definition.GetDecisionSpace().Equals(optimalParetoFrontMembers.First().GetDecisionSpace());
        }
        
        #region ToString
        public override string ToString()
        {
            return $"{name} ({optimalParetoFrontMembers.First().Count} dimensions, {numberOfObjectives} objectives)";
        }
        #endregion
    }
}