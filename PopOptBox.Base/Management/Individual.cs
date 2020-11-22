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
    /// Immutable by design.
    /// </summary>
    public class Individual
    {
        #region Fields

        /// <summary>
        /// Definition of the individual
        /// </summary>
        public readonly DecisionVector DecisionVector;

        /// <summary>
        /// This contains any additional information a process wishes to store.
        /// </summary>
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

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
        /// Whether the individual is legal or not.
        /// </summary>
        public bool Legal { get; private set; }

        /// <summary>
        /// Current state of the individual.
        /// </summary>
        public IndividualState State { get; private set; } = IndividualState.New;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct individual.
        /// </summary>
        /// <param name="decisionVector">The numerical definition of the individual for the optimisation</param>
        public Individual(DecisionVector decisionVector)
        {
            DecisionVector = decisionVector;
            SolutionVector = new double[0];
        }

        /// <summary>
        /// Returns a shallow copy of the Individual
        /// </summary>
        public Individual Clone()
        {
            var newIndividual = new Individual(DecisionVector)
            {
                SolutionVector = (double[])SolutionVector.Clone(),
                Fitness = Fitness,
                Legal = Legal,
                State = State,
            };
            
            foreach (var key in properties.Keys)
            {
                newIndividual.SetProperty(key, properties[key]);
            }

            return newIndividual;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Returns the Decision Vector as a string.
        /// </summary>
        /// <returns>A string version of <see cref="DecisionVector"/>.</returns>
        public override string ToString()
        {
            return $"{Fitness} [{string.Join(" - ", DecisionVector)}]";
        }

        /// <summary>
        /// Gets all the keys currently stored in the properties.
        /// </summary>
        /// <returns>A list of property names.</returns>
        // ReSharper disable once UnusedMember.Global - API function
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
        /// Retrieves the value for a given property key.
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
        /// Removes the specified property from the property dictionary.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Dictionary{string, object}.Remove(string)"/>: key does not need to exist.
        /// </remarks>
        /// <param name="key">The key for the property to remove.</param>
        public void RemoveProperty(string key)
        {
            properties.Remove(key);
        }

        /// <summary>
        /// Assigns Solution Vector based on given Property names and sets State to <see cref="IndividualState.Evaluated"/>.
        /// </summary>
        /// <param name="keyNames">Names of property keys to set as solution vector.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any property name does not exist.</exception>
        public void SetSolution(params string[] keyNames)
        {
            if (State != IndividualState.Evaluating)
                throw new InvalidOperationException("Individual is not evaluating!");

            SolutionVector = keyNames.Select(GetProperty<double>).ToArray();

            Legal = true;
            State = IndividualState.Evaluated;
        }

        /// <summary>
        /// Assigns Fitness value.
        /// </summary>
        /// <remarks>Can be performed more than once.</remarks>
        /// <param name="fitness">
        /// The fitness value which has been calculated by the <see cref="Optimiser"/>.
        /// <seealso cref="PopOptBox.Base.FitnessCalculation.IFitnessCalculator"/>
        /// </param>
        /// <exception cref="InvalidOperationException">Thrown when Individual is not evaluated. <seealso cref="SetSolution"/>.</exception>
        public void SetFitness(double fitness)
        {
            if (State != IndividualState.Evaluated && State != IndividualState.FitnessAssessed)
                throw new InvalidOperationException("Individual is not evaluated!");

            Fitness = fitness;

            State = IndividualState.FitnessAssessed;
        }

        /// <summary>
        /// Sets the individual as illegal and the state as <see cref="IndividualState.Evaluated"/>.
        /// </summary>
        public void SetIllegal()
        {
            if (State != IndividualState.Evaluating)
                throw new InvalidOperationException("Individual is not evaluating!");

            Legal = false;
            State = IndividualState.Evaluated;
        }

        #endregion

        #region Equals, GetHashCode

        public override bool Equals(object obj)
        {
            if (!(obj is Individual other))
                return false;

            return DecisionVector.Equals(other.DecisionVector) &&
                   SolutionVector.SequenceEqual(other.SolutionVector);
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