using System.Linq;
using MathNet.Numerics.Random;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;
using PopOptBox.Problems.SingleObjective.Continuous;

namespace PopOptBox.HyperParameterTuning.Helpers.Continuous
{
    public class ContinuousProblemModel : Model<DecisionVector>
    {
        private RandomSource rng;
        private DecisionSpace decisionSpace;
        
        public ContinuousProblemModel(DecisionSpace decisionSpace) 
            : base(new ContinuousProblemConverter(decisionSpace), ContinuousProblemPropertyNames.TheLocation)
        {
            rng = new MersenneTwister(true);
            this.decisionSpace = decisionSpace;
        }

        public override DecisionVector GetNewDecisionVector()
        {
            var newVector = decisionSpace.Select(d => (double) d.GetNextRandom(rng));
            return DecisionVector.CreateFromArray(decisionSpace, newVector);
        }
    }
}