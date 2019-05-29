using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Core.Population
{
    public class DecisionVector
    {
        private DecisionSpace decisionSpace;
        
        // Definition of the individual for the optimiser
        public readonly IReadOnlyList<object> Vector;

        public DecisionVector(DecisionSpace decisionSpace,
            IEnumerable<object> values)
        {
            this.decisionSpace = decisionSpace;
            Vector = values.ToArray();
        }
    }
}