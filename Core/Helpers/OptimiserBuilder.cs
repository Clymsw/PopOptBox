using System;
using System.Collections.Generic;
using Optimisation.Base.Conversion;
using Optimisation.Base.Management;
using Optimisation.Base.Variables;

namespace Optimisation.Base.Helpers
{
    /// <summary>
    /// Helper class for creating a full optimisation environment
    /// </summary>
    public abstract class OptimiserBuilder
    {
        private readonly Dictionary<IVariable, object> settings;

        protected OptimiserBuilder()
        {
            settings = new Dictionary<IVariable, object>();
        }

        public IEnumerable<IVariable> GetTunableSettings()
        {
            return settings.Keys;
        }

        /// <summary>
        /// Allows setting an optimisation hyperparameter
        /// </summary>
        /// <param name="definition">Setting definition in form of a <see cref="IVariable"/></param>
        /// <param name="value">The value for the setting</param>
        /// <returns><see langword="true" /> if set ok</returns>
        public bool SetSetting(IVariable definition, object value)
        {
            try
            {
                settings[definition] = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     The optimiser
        /// </summary>
        /// <param name="decVecGenerator"></param>
        /// <param name="solToFit"></param>
        /// <param name="penaltyFcn"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public abstract Optimiser CreateOptimiser();

        /// <summary>
        ///     The model
        /// </summary>
        /// <returns></returns>
        public abstract Model CreateModel();

        /// <summary>
        ///     The Decision Vector generator for population initialisation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected Func<Individual> CreateDecisionVectorGenerator()
        {
            var model = CreateModel();
            return model.GetNewIndividual;
        }

        /// <summary>
        ///     The score to fitness converter
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreateObjective();
        
        /// <summary>
        ///     The solution vector to score converter
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double[]> CreateMultiObjectiveScore();

        /// <summary>
        ///     The solution vector to penalty converter for illegal individuals
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreatePenalty();
    }
}