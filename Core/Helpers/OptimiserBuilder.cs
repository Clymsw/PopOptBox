using System;
using System.Collections.Generic;
using Optimisation.Core.Model;
using Optimisation.Core.Optimiser;
using Optimisation.Core.Population;

namespace Optimisation.Core.Helpers
{
    /// <summary>
    ///     Helper class for creating a full optimisation environment
    /// </summary>
    public abstract class OptimiserBuilder
    {
        private readonly Dictionary<string, object> Settings;

        protected OptimiserBuilder()
        {
            Settings = new Dictionary<string, object>();
        }

        public virtual IEnumerable<KeyValuePair<string, object>> GetTunableSettings()
        {
            return Settings;
        }

        public virtual bool SetSetting(string name, object newValue)
        {
            try
            {
                Settings[name] = newValue;
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
        public abstract Optimiser.Optimiser CreateOptimiser();

        /// <summary>
        ///     The model
        /// </summary>
        /// <returns></returns>
        public abstract Model.Model CreateModel();

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
        ///     The solution vector to fitness converter
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreateObjective();

        /// <summary>
        ///     The solution vector to penalty converter for illegal individuals
        /// </summary>
        /// <returns></returns>
        protected abstract Func<double[], double> CreatePenalty();
    }
}