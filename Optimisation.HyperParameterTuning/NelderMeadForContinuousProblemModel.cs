using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;
using PopOptBox.Problems.SingleObjective.Continuous;

namespace PopOptBox.HyperParameterTuning
{
    public class NelderMeadForContinuousProblemModel : Model<DecisionVector>
    {
        private RandomSource rng;
        private DecisionSpace decisionSpace;
        
        public NelderMeadForContinuousProblemModel(DecisionSpace decisionSpace) 
            : base(new ContinuousProblemConverter(decisionSpace), ContinuousProblemPropertyNames.TheLocation)
        {
            rng = new MersenneTwister();
            this.decisionSpace = decisionSpace;
        }

        public override DecisionVector GetNewDecisionVector()
        {
            var newVector = decisionSpace.Dimensions.Select(d => (double) d.GetNextRandom(rng));
            return DecisionVector.CreateFromArray(decisionSpace, newVector);
        }
    }
}