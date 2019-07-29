namespace Optimisation.Optimisers.EvolutionaryComputation
{
    /// <summary>
    /// Abstract for all operators, should be inherited together with e.g. <seealso cref="IMutationOperator"/>.
    /// </summary>
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
