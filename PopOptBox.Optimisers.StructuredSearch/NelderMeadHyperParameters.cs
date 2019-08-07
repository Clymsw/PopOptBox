using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;

namespace PopOptBox.Optimisers.StructuredSearch
{
    public static class NelderMeadHyperParameters
    {
        public const string Simplex_Creation_Step_Size = "Simplex creation step size";
        public const string Reflection_Coefficient = "Reflection Coefficient";
        public const string Expansion_Coefficient = "Expansion Coefficient";
        public const string Contraction_Coefficient = "Contraction Coefficient";
        public const string Shrinkage_Coefficient = "Shrinkage Coefficient";

        public static HyperParameterManager GetDefaultHyperParameters()
        {
            var mgr = new HyperParameterManager();
            
            mgr.AddOrReplaceHyperParameter(
                new VariableContinuous(0.001, 100,
                    0.001, 100,
                    Simplex_Creation_Step_Size),
                0.5);
            
            mgr.AddOrReplaceHyperParameter(
                new VariableContinuous(0.001, double.MaxValue,
                    0.5, 2,
                    Reflection_Coefficient),
                1.0);
            
            mgr.AddOrReplaceHyperParameter(
                new VariableContinuous(1.001, double.MaxValue,
                    1.1, 5,
                    Expansion_Coefficient),
                2.0);

            mgr.AddOrReplaceHyperParameter(
                new VariableContinuous(0.001, 1.0,
                    0.01, 0.9,
                    Contraction_Coefficient),
                0.5);
            
            mgr.AddOrReplaceHyperParameter(
                new VariableContinuous(0.001, 1.0,
                    0.01, 0.9,
                    Shrinkage_Coefficient),
                0.5);
            
            return mgr;
        }
    }
}