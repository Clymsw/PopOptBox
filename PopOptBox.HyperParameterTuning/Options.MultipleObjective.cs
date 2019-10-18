using System;
using PopOptBox.Problems.MultipleObjective;
using PopOptBox.Problems.MultipleObjective.Continuous;

namespace PopOptBox.HyperParameterTuning
{
    internal static partial class Options
    {
        #region Optimiser
        
        public enum ProblemsMultipleObjectiveContinuousAvailable
        {
            Zdt1,
            Zdt2,
            Zdt3
        }
        
        public static ProblemMultipleObjective GetMultipleObjectiveContinuousProblem(
            ProblemsMultipleObjectiveContinuousAvailable problem)
        {
            switch (problem)
            {
                case ProblemsMultipleObjectiveContinuousAvailable.Zdt1:
                    return new Zdt1();
                
                case ProblemsMultipleObjectiveContinuousAvailable.Zdt2:
                    return new Zdt2();
                
                case ProblemsMultipleObjectiveContinuousAvailable.Zdt3:
                    return new Zdt3();

                default:
                    throw new NotImplementedException();
            }
        }
                
        #endregion
    }
}