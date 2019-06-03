using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Variables
{
    /// <summary>
    /// TODO!
    /// </summary>
    public class DecisionVector
    {
        private readonly DecisionSpace decisionSpace;

        // Definition of the individual for the optimiser
        public readonly IReadOnlyList<object> Vector;

        #region Constructor

        private DecisionVector(DecisionSpace decisionSpace,
            object[] values)
        {
            this.decisionSpace = decisionSpace;

            //Check that all values are sensible
            if (!decisionSpace.IsAcceptableDecisionVector(values))
                throw new System.ArgumentOutOfRangeException(nameof(values),
                    "Vector values are not accepted by the decision space");

            Vector = values.ToArray();
        }

        public static DecisionVector CreateFromItems(DecisionSpace decisionSpace,
            params object[] values)
        {
            return new DecisionVector(decisionSpace, values);
        }

        public static DecisionVector CreateFromArray(DecisionSpace decisionSpace,
            IEnumerable<int> values)
        {
            return new DecisionVector(decisionSpace, values.Cast<object>().ToArray());
        }

        public static DecisionVector CreateFromArray(DecisionSpace decisionSpace,
            IEnumerable<double> values)
        {
            return new DecisionVector(decisionSpace, values.Cast<object>().ToArray());
        }

        #endregion

        // TODO: Extend this to retrieve the elements of each type, based on the decision space.
    }
}