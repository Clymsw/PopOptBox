using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;

namespace Optimisation.HyperParameterTuning
{
    internal class ContinuousProblemConverter : IConverter<DecisionVector>
    {
        private readonly DecisionSpace space;
        
        public ContinuousProblemConverter(DecisionSpace space)
        {
            this.space = space;
        }
        
        public DecisionVector ConvertToDv(DecisionVector realityDefinition)
        {
            return realityDefinition;
        }

        public DecisionVector ConvertToReality(DecisionVector decisionVector)
        {
            return decisionVector;
        }
    }
}
