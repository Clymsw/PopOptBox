using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;
using System;
using System.Linq;

namespace Optimisation.Problems.Continuous
{
    internal class ProblemConverter : IConverter<double[]>
    {
        public DecisionVector ConvertToDv(double[] realityDefinition)
        {
            throw new NotImplementedException();
        }

        public double[] ConvertToReality(DecisionVector decisionVector)
        {
            return decisionVector.Vector.Select(Convert.ToDouble).ToArray();
        }
    }
}
