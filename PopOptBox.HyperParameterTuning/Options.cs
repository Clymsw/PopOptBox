using System;
using PopOptBox.Base.Management;
using PopOptBox.Base.Variables;
using PopOptBox.HyperParameterTuning.SingleObjective.Continuous.NelderMead;
using PopOptBox.Optimisers.StructuredSearch;
using PopOptBox.Problems.SingleObjective.Continuous;

namespace PopOptBox.HyperParameterTuning
{
    internal static class Options
    {
        #region Optimiser
        
        public enum OptimisersAvailable
        {
            NelderMead,
            EvolutionaryAlgorithm
        }

        #endregion
        
        #region Problems

        public enum ProblemsSingleObjectiveContinuousAvailable
        {
            Ellipsoidal,
            Schwefel,
            Rosenbrock,
            StyblinskiTang,
            Rastrigin,
            Salomon
        }
        
        public static ProblemSingleObjectiveContinuous GetProblem(
            ProblemsSingleObjectiveContinuousAvailable problem,
            int numberOfDimensions)
        {
            switch (problem)
            {
                case ProblemsSingleObjectiveContinuousAvailable.Ellipsoidal:
                    return new Ellipsoidal(numberOfDimensions);
                
                case ProblemsSingleObjectiveContinuousAvailable.Schwefel:
                    return new Schwefel(numberOfDimensions);
                
                case ProblemsSingleObjectiveContinuousAvailable.Rosenbrock:
                    return new Rosenbrock(numberOfDimensions);
                
                case ProblemsSingleObjectiveContinuousAvailable.StyblinskiTang:
                    return new StyblinskiTang(numberOfDimensions);
                
                case ProblemsSingleObjectiveContinuousAvailable.Rastrigin:
                    return new Rastrigin(numberOfDimensions);
                
                case ProblemsSingleObjectiveContinuousAvailable.Salomon:
                    return new Salomon(numberOfDimensions);
                
                default:
                    throw new NotImplementedException();
            }
        }
                
        #endregion
    }
}