using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;

namespace PopOptBox.HyperParameterTuning
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
