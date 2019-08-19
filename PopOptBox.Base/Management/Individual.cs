using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Variables;

namespace PopOptBox.Base.Management
{
    /// <summary>
    /// "Individual" class, which contains information about 
    /// a given single solution for an optimisation problem.
    /// </summary>
    public class Individual
    {
        #region Fields

        /// <summary>
        /// Definition of the individual
        /// </summary>
        public readonly DecisionVector DecisionVector;

        /// <summary>
        /// This contains any numeric information an evaluator wishes to store.
        /// </summary>
        /// <remarks>Not readonly to enable Individual cloning</remarks>
        private Dictionary<string, object> properties =
            new Dictionary<string, object>();

        /// <summary>
        /// The solution(s) currently relevant for the optimisation.
        /// </summary>
        public double[] SolutionVector { get; private set; }

        /// <summary>
        /// If the individual is legal or not.
        /// </summary>
        public bool Legal { get; private set; }

        /// <summary>
        /// Current state of the individual.
        /// </summary>
        public IndividualState State = IndividualState.New;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct individual, immutable by design.
        /// </summary>
        /// <param name="decisionVector">The numerical definition of the individual for the optimisation</param>
        public Individual(DecisionVector decisionVector)
        {
            DecisionVector = decisionVector;
        }

        /// <summary>
        /// Returns a shallow copy of the Individual
        /// </summary>
        public Individual Clone()
        {
            var propCopy = new Dictionary<string, object>();
            foreach (var key in properties.Keys)
            {
                propCopy.Add(key, properties[key]);
            }

            return new Individual(DecisionVector)
            {
                SolutionVector = (double[])SolutionVector?.Clone(),
                properties = propCopy,
                Legal = Legal,
                State = State,
            };
        }

        #endregion

        #region Utility

        /// <summary>
        /// Returns the Decision Vector as a string.
        /// </summary>
        /// <returns>A string version of <see cref="DecisionVector"/>.</returns>
        public override string ToString()
        {
            return string.Join(" - ", DecisionVector);
        }

        /// <summary>
        /// Gets all the keys currently stored in the properties.
        /// </summary>
        /// <returns>A list of property names.</returns>
        public IEnumerable<string> GetPropertyNames()
        {
            var keynames = properties.Keys;
            return keynames;
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Call when sending for evaluation.
        /// </summary>
        /// <remarks>Managed automatically by <see cref="Model{TReality}"/>.</remarks>
        /// <exception cref="InvalidOperationException">Individual is not new.</exception>
        internal void SendForEvaluation()
        {
            if (State == IndividualState.New)
                State = IndividualState.Evaluating;
            else
                throw new InvalidOperationException("Individual is not new!");
        }

        /// <summary>
        /// Stores a Key-Value pair in the individual's properties.
        /// If key already exists, value is over-written.
        /// Use <seealso cref="GetPropertyNames"/> to find existing property names.
        /// </summary>
        /// <param name="key">Property name.</param>
        /// <param name="value">Property value.</param>
        public void SetProperty(string key, object value)
        {
            if (properties.ContainsKey(key))
            {
                // TODO: Use logger to send warning.
                properties[key] = value;
            }
            else
            {
                properties.Add(key, value);
            }
        }

        /// <summary>
        /// Retrieves the value for a given property key, if it exists.
        /// Otherwise returns a zero element double array and a warning.
        /// </summary>
        /// <param name="key">String acting as key for the property</param>
        /// <returns>Property value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when key is not present.</exception>
        public T GetProperty<T>(string key)
        {
            if (!properties.TryGetValue(key, out var value))
                throw new ArgumentOutOfRangeException(nameof(key), key, "Key not found");
            return (T) value;
        }

        /// <summary>
        /// Assigns Solution Vector based on a given Property name
        /// </summary>
        /// <param name="keyName">Name of property key</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when property name does not exist.</exception>
        public void SetSolution(string keyName)
        {
            if (State != IndividualState.Evaluating)
                throw new InvalidOperationException("Individual is not evaluating!");

            var solutionValue = GetProperty<double[]>(keyName);
            SolutionVector = solutionValue ??
                             throw new ArgumentOutOfRangeException(nameof(keyName),
                                 "Invalid key provided to get solution!");

            State = IndividualState.Evaluated;
        }

        /// <summary>
        /// Sets the legality of the individual.
        /// </summary>
        /// <param name="legal">Set <see langword="true"/> when the individual is legal.</param>
        public void SetLegality(bool legal)
        {
            Legal = legal;
        }

        #endregion

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is Individual other))
                return false;

            return DecisionVector.Equals(other.DecisionVector) &&
                   (SolutionVector?.SequenceEqual(other.SolutionVector) ?? true);
        }

        public override int GetHashCode()
        {
            return new
            {
                DecisionVector
            }.GetHashCode();
        }

        #endregion
    }
}