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
        private readonly int numberOfObjectives;
        protected readonly DecisionSpace decisionSpace;
        
        protected ProblemMultipleObjective(string name, DecisionSpace decisionSpace, 
            string definitionKey, params string[] solutionKeys) 
            : base(definitionKey, solutionKeys)
        {
            this.name = name;
            numberOfObjectives = solutionKeys.Length;
            this.decisionSpace = decisionSpace;
        }

        /// <summary>
        /// Gets a number of locations specifying the optimal Pareto Front.
        /// </summary>
        /// <param name="numberOfPoints">The maximum number of points on the Pareto Front to return.</param>
        /// <returns>An array of <see cref="DecisionVector"/>s.</returns>
        public abstract DecisionVector[] GetOptimalParetoFront(int numberOfPoints);

        public override bool GetLegality(DecisionVector definition)
        {
            // By definition, if we can construct a Decision Vector in the same decision space, it is legal
            return definition.GetDecisionSpace().Equals(decisionSpace);
        }
        
        #region ToString
        public override string ToString()
        {
            return $"{name} ({decisionSpace.Count} dimensions, {numberOfObjectives} objectives)";
        }
        #endregion
    }
}