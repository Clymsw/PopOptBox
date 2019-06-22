using Optimisation.Base.Conversion;
using Optimisation.Base.Management;
using System;

namespace Optimisation.Problems.Continuous
{
    internal class ProblemModel : Model
    {
        public ProblemModel() : base(
            new ProblemConverter(), 
            ContinuousProblemDefinitions.TheLocation)
        {
        }

        protected override Individual CreateNewIndividual()
        {
            throw new NotImplementedException();
        }
    }
}
