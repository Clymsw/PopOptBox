using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;
using System;
using System.Linq;

namespace Optimisation.Problems.Continuous
{
    internal class ProblemConverter : IConverter
    {
        public DecisionVector ConvertToDV(object realityDefinition)
        {
            throw new NotImplementedException();
        }

        public object ConvertToReality(DecisionVector decisionVector)
        {
            return decisionVector.Vector.Select(Convert.ToDouble).ToArray();
        }
    }
}
