using Optimisation.Base.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimisation.Base.Management
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
        /// The Fitness, used to rank individuals and ultimately determine optimality.
        /// Lower is better.
        /// </summary>
        public double Fitness { get; private set; }

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
                SolutionVector = (double[]) SolutionVector?.Clone(),
                Fitness = Fitness,
                properties = propCopy,
                Legal = Legal,
                State = State
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
            return $"{Fitness} [{string.Join(" - ", DecisionVector.Vector)}]";
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
        /// <remarks>Managed automatically by <see cref="Conversion.Model"/>.</remarks>
        /// <exception cref="InvalidOperationException">Individual is not new.</exception>
        internal void SendForEvaluation()
        {
            if (State == IndividualState.New)
                State = IndividualState.Evaluating;
            else
                throw new InvalidOperationException("Individual is not new!");
        }

        /// <summary>
        /// Call when finished evaluating.
        /// </summary>
        /// <remarks>Managed automatically by <see cref="Conversion.Evaluator"/>.</remarks>
        /// <exception cref="InvalidOperationException">Individual is not evaluating.</exception>
        internal void FinishEvaluating()
        {
            if (State == IndividualState.Evaluating)
                State = IndividualState.Evaluated;
            else
                throw new InvalidOperationException("Individual is not evaluating!");
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
            var solutionValue = GetProperty<double[]>(keyName);
            SolutionVector = solutionValue ??
                             throw new ArgumentOutOfRangeException(nameof(keyName),
                                 "Invalid key to set solution!");
        }

        /// <summary>
        /// Assigns Fitness based on a function which must be passed
        /// in as a delegate that converts a double array into a single value.
        /// </summary>
        /// <param name="solutionToFitness">Delegate converting Solution Vector to Fitness.</param>
        /// <exception cref="InvalidOperationException">Thrown when Solution Vector is null. <seealso cref="SetSolution(string)"/>.</exception>
        public void SetFitness(Func<double[], double> solutionToFitness)
        {
            Fitness = solutionToFitness(SolutionVector);
        }

        /// <summary>
        /// Sets the legality of the individual.
        /// </summary>
        /// <param name="legal">Set <see langword="true"/> when the individual is legal.</param>
        public void SetLegality(bool legal)
        {
            Legal = legal;
        }

        /// <summary>
        /// Gets whether another Individual strictly dominates this one.
        /// </summary>
        /// <param name="other">The other Individual to compare.</param>
        /// <returns><see langword="true"/> if this individual is dominated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the two Solution Vectors have different lengths.</exception>
        public bool IsDominatedBy(Individual other)
        {
            if (other.SolutionVector.Length != SolutionVector.Length)
                throw new ArgumentOutOfRangeException(nameof(other), 
                    "Other individual must have the same number of objectives in its Solution Vector.");
            
            return other.SolutionVector.Select((v, i) => v < SolutionVector.ElementAt(i)).All(b => b);
        }

        /// <summary>
        /// Gets whether this Individual strictly dominates another one.
        /// </summary>
        /// <param name="other">The other Individual to compare.</param>
        /// <returns><see langword="true"/> if the other individual is dominated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the two Solution Vectors have different lengths.</exception>
        public bool Dominates(Individual other)
        {
            if (other.SolutionVector.Length != SolutionVector.Length)
                throw new ArgumentOutOfRangeException(nameof(other), 
                    "Other individual must have the same number of objectives in its Solution Vector.");
            
            return SolutionVector.Select((v, i) => v < other.SolutionVector.ElementAt(i)).All(b => b);
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