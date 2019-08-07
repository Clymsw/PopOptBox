using System;
using System.Linq;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Helper class for creating a full optimisation environment
    /// </summary>
    public abstract class OptimiserBuilder
    {
        private DecisionVector hyperParameters;

        protected OptimiserBuilder()
        {
            hyperParameters = DecisionVector.CreateForEmpty();
        }

        /// <summary>
        /// All the hyperparameter definitions and values.
        /// </summary>
        public DecisionVector HyperParameters => hyperParameters;

        /// <summary>
        /// Returns the value of a particular hyperparameter.
        /// </summary>
        /// <param name="name">The <see cref="IVariable"/> Name defining the hyperparameter.</param>
        /// <returns>An object value for the hyperparameter.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the setting is not known.</exception>
        public object GetHyperParameterValue(string name)
        {
            return hyperParameters
                .Where((v, i) => hyperParameters.GetDecisionSpace().ElementAt(i).Name == name)
                .Single(); 
        }

        /// <summary>
        /// Allows adding an optimisation hyperparameter.
        /// </summary>
        /// <param name="definition">Hyperparameter definition, in the form of an <see cref="IVariable"/>.</param>
        /// <param name="value">The value for the setting</param>
        /// <returns><see langword="true" /> if set ok</returns>
        public bool AddHyperParameter(IVariable definition, object value)
        {
            try
            {
                var decSpace = hyperParameters.GetDecisionSpace().ToList();
                decSpace.Add(definition);
                var values = hyperParameters.ToList();
                values.Add(value);
                
                hyperParameters = DecisionVector.CreateFromArray(
                    new DecisionSpace(decSpace),
                    values);
                
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