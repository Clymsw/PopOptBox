using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation
{
    public abstract class Operator
    {
        private readonly string name;
        
        public Operator(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
