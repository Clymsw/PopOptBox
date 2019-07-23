using Optimisation.Base.Variables;

namespace Optimisation.Optimisers.EvolutionaryComputation.Mutation
{
    public abstract class MutationOperator : IMutationOperator
    {
        private readonly string name;
        
        public MutationOperator(string name)
        {
            this.name = name;
        }

        public abstract DecisionVector Operate(DecisionVector decisionVector);

        public override string ToString()
        {
            return name;
        }
    }
}
