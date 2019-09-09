using System;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.FitnessCalculation;
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
        ///     The Solution Vector to Fitness converter passed into the <see cref="Optimiser"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract IFitnessCalculator CreateFitnessCalculator();
    }
}