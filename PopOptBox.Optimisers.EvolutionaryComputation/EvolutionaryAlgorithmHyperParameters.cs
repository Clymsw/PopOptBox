using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.EvolutionaryComputation
{
    public static class EvolutionaryAlgorithmHyperParameters
    {
        public const string Population_Size = "Population Size";
        public const string Number_Of_Parents = "Number of Parents";

        public static HyperParameterManager GetDefaultHyperParameters()
        {
            var mgr = new HyperParameterManager();
            
            mgr.AddOrReplaceHyperParameter(
                new VariableDiscrete(5, 10000,
                    10, 1000,
                    Population_Size), 
                100);
            
            mgr.AddOrReplaceHyperParameter(
                new VariableDiscrete(2, 1000,
                    2, 200,
                    Number_Of_Parents), 
                40);
            
            return mgr;
        }
    }
}