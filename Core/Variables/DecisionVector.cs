using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Variables
{
    public class DecisionVector
    {
        private readonly DecisionSpace decisionSpace;
        
        // Definition of the individual for the optimiser
        public readonly IReadOnlyList<object> Vector;

        public DecisionVector(DecisionSpace decisionSpace,
            params object[] values)
        {
            this.decisionSpace = decisionSpace;
            //TODO: Check that they are sensible!
            Vector = values.ToArray();
        }

        // TODO: Extend this to retrieve the elements of each type, based on the decision space.
    }
}