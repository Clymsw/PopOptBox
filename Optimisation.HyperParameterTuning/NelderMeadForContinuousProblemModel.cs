using System.Linq;
using MathNet.Numerics.Random;
using Optimisation.Base.Conversion;
using Optimisation.Base.Variables;
using Optimisation.Problems.SingleObjective.Continuous;

namespace Optimisation.HyperParameterTuning
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

        protected override DecisionVector CreateNewIndividual()
        {
            var newVector = decisionSpace.Dimensions.Select(d => (double) d.GetNextRandom(rng));
            return DecisionVector.CreateFromArray(decisionSpace, newVector);
        }
    }
}