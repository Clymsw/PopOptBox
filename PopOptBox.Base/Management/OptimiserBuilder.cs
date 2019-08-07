using System;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Helper class for creating a full optimisation environment
    /// </summary>
    public abstract class OptimiserBuilder
    {
        public HyperParameterManager HyperParameters { get; }

        protected OptimiserBuilder()
        {
            HyperParameters = new HyperParameterManager();
        }

        /// <summary>
        ///     The optimiser
        /// </summary>
        /// <returns></returns>
        public abstract Optimiser CreateOptimiser();

        /// <summary>
        ///     The model
        /// </summary>
        /// <returns></returns>
        public abstract IModel CreateModel();

        /// <summary>
        ///     The Decision Vector generator for population initialisation
        /// </summary>
        /// <returns></returns>
        protected Func<DecisionVector> CreateDecisionVectorGenerator()
        {
            var model = CreateModel();
            return model.GetNewDecisionVector;
        }

        /// <summary>
        ///     The solution to Fitness converter
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreateSolutionToFitness();

        /// <summary>
        ///     The Solution Vector to penalty converter for illegal individuals
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreatePenalty();
    }
}