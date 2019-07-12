using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;
using System;
using System.Linq;

namespace Optimisation.HyperParameterTuning
{
    internal class ContinuousProblemConverter : IConverter<double[]>
    {
        private readonly DecisionSpace space;
        
        public ContinuousProblemConverter(DecisionSpace space)
        {
            this.space = space;
        }
        
        public DecisionVector ConvertToDv(double[] realityDefinition)
        {
            return DecisionVector.CreateFromArray(space, realityDefinition);
        }

        public double[] ConvertToReality(DecisionVector decisionVector)
        {
            return decisionVector.Vector.Select(Convert.ToDouble).ToArray();
        }
    }
}
