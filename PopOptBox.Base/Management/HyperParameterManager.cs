using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// Manages hyperparameters (i.e. settings) for <see cref="Optimiser"/>s, and parts of optimisers.
    /// </summary>
    public class HyperParameterManager
    {
        /// <summary>
        /// The array of hyperparameters, stored as a <see cref="DecisionVector"/>.
        /// </summary>
        public DecisionVector HyperParameters { get; private set; }

        /// <summary>
        /// Creates an empty hyperparameter manager. 
        /// </summary>
        public HyperParameterManager()
        {
            HyperParameters = DecisionVector.CreateForEmpty();
        }
        
        /// <summary>
        /// Gets the names of all the hyperparameters stored.
        /// </summary>
        /// <returns></returns>
        public string[] GetHyperParameterNames()
        {
            return HyperParameters.GetDecisionSpace().Select(v => v.Name).ToArray();
        }
        
        /// <summary>
        /// Returns the value of a particular hyperparameter.
        /// </summary>
        /// <param name="name">The <see cref="IVariable"/> Name defining the hyperparameter.</param>
        /// <returns>An object value for the hyperparameter.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the setting is not known.</exception>
        public T GetHyperParameterValue<T>(string name)
        {
            return (T)HyperParameters
                .Where((v, i) => HyperParameters.GetDecisionSpace().ElementAt(i).Name == name)
                .Single(); 
        }

        /// <summary>
        /// Combines two sets of hyperparameters together, by adding from another manager into this one.
        /// </summary>
        /// <param name="other">The other <see cref="HyperParameterManager"/>.</param>
        /// <returns>The number of hyperparameters added.</returns>
        public int AddFromExistingHyperParameterSet(HyperParameterManager other)
        {
            var numAdded = 0;
            
            for (var i = 0; i < other.HyperParameters.Count; i++)
            {
                if (AddOrReplaceHyperParameter(
                    other.HyperParameters.GetDecisionSpace().ElementAt(i),
                    other.HyperParameters[i]))
                    numAdded++;
            }

            return numAdded;
        }
        
        /// <summary>
        /// Adds an optimisation hyperparameter, or replaces an existing one.
        /// </summary>
        /// <param name="definition">Hyperparameter definition, in the form of an <see cref="IVariable"/>.</param>
        /// <param name="value">The value for the hyperparameter.</param>
        /// <returns><see langword="true" /> if set ok.</returns>
        public bool AddOrReplaceHyperParameter(IVariable definition, object value)
        {
            try
            {
                var decSpace = HyperParameters.GetDecisionSpace().ToList();
                var values = HyperParameters.ToList();
                var names = GetHyperParameterNames();
                
                if (names.Contains(definition.Name))
                {
                    // It exists: remove it first.
                    var idx = Array.IndexOf(names, definition.Name);
                    decSpace.RemoveAt(idx);
                    values.RemoveAt(idx);
                }

                decSpace.Add(definition);
                values.Add(value);

                return replaceHyperParametersFrom(decSpace, values);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Replaces the value of an existing hyperparameter.
        /// </summary>
        /// <param name="name">Hyperparameter name.</param>
        /// <param name="value">The value for the hyperparameter.</param>
        /// <returns><see langword="true" /> if set ok.</returns>
        public bool UpdateHyperParameterValue(string name, object value)
        {
            var names = GetHyperParameterNames();
            
            if (!names.Contains(name))
                return false;
            
            try
            {
                var decSpace = HyperParameters.GetDecisionSpace().ToList();
                var values = HyperParameters.ToList();
                var idx = Array.IndexOf(names, name);
                values[idx] = value;

                return replaceHyperParametersFrom(decSpace, values);
            }
            catch
            {
                return false;
            }
        }

        private bool replaceHyperParametersFrom(IEnumerable<IVariable> decisionSpace, IEnumerable<object> values)
        {
            HyperParameters = DecisionVector.CreateFromArray(
                new DecisionSpace(decisionSpace),
                values);
                
            return true;
        }
    }
}