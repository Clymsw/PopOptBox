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
        /// The solution(s) currently relevant for the optimisation
        /// </summary>
        public double[] SolutionVector { get; private set; }

        /// <summary>
        /// Score for multi-objective optimisation with domination
        /// </summary>
        public double[] Score { get; private set; }

        /// <summary>
        /// The fitness, used to rank individuals and ultimately determine optimality.
        /// Lower is better.
        /// </summary>
        public double Fitness { get; private set; }

        /// <summary>
        /// If the individual is legal or not
        /// </summary>
        public bool Legal { get; private set; }

        /// <summary>
        /// Current state of the individual
        /// </summary>
        public IndividualStates State = IndividualStates.New;

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
                Score = (double[])Score?.Clone(),
                Fitness = Fitness,
                properties = propCopy,
                Legal = Legal,
                State = State
            };
        }

        #endregion

        #region Utility

        /// <summary>
        /// Returns the decision vector as a string
        /// </summary>
        /// <returns>string version of DV</returns>
        public override string ToString()
        {
            return $"{Fitness} [{string.Join(", ", DecisionVector)}]";
        }

        /// <summary>
        /// Obtain all the keys currently stored in the properties
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNames()
        {
            var keynames = properties.Keys;
            return keynames;
        }

        #endregion

        #region Runtime

        /// <summary>
        /// Call when sending for evaluation.
        /// Managed automatically by <see cref="Optimisation.Base.Conversion.Model"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Individual is not new</exception>
        internal void SendForEvaluation()
        {
            if (State == IndividualStates.New)
                State = IndividualStates.Evaluating;
            else
                throw new InvalidOperationException("Individual is not new!");
        }

        /// <summary>
        /// Call when finished evaluating.
        /// Managed automatically by <see cref="Optimisation.Base.Conversion.Evaluator"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Individual is not evaluating</exception>
        internal void FinishEvaluating()
        {
            if (State == IndividualStates.Evaluating)
                State = IndividualStates.Evaluated;
            else
                throw new InvalidOperationException("Individual is not evaluating!");
        }

        /// <summary>
        /// Stores a Key-Value pair in the individual's properties.
        /// If key already exists, value is over-written with a warning.
        /// Use <see cref="GetPropertyNames"/> to find existing strings.
        /// </summary>
        /// <param name="key">String acting as key for the property</param>
        /// <param name="value">Value of the property</param>
        public void SetProperty(string key, object value)
        {
            if (properties.ContainsKey(key))
            {
                // TODO: Use logger
                Console.WriteLine("Warning: Over-writing property '" + key + "'");
                var oldValue = properties[key];
                Console.WriteLine("Old value: " + oldValue);
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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when key is not present.</exception>
        /// <returns>Property value</returns>
        public T GetProperty<T>(string key)
        {
            if (!properties.TryGetValue(key, out var value))
                throw new ArgumentOutOfRangeException(nameof(key), key, "Key not found");
            return (T) value;
        }

        /// <summary>
        /// Assigns Solution Vector based on a given Property key value
        /// </summary>
        /// <param name="keyName">Name of property key</param>
        public void SetSolution(string keyName)
        {
            var solutionValue = GetProperty<double[]>(keyName);
            SolutionVector = solutionValue ??
                             throw new ArgumentOutOfRangeException(nameof(keyName), 
                                 "Invalid key to set solution!");
        }

        /// <summary>
        /// Assigns Score based on a function which must be passed
        /// in as a delegate that converts a double array into a single value
        /// </summary>
        /// <param name="solToScore">Delegate converting solution to score</param>
        public void SetScore(Func<double[], double[]> solToScore)
        {
            var scoreValue = solToScore(SolutionVector);
            Score = scoreValue;
        }

        /// <summary>
        /// Assigns Fitness based on a function which must be passed
        /// in as a delegate that converts a double array into a single value
        /// </summary>
        /// <remarks>Will bypass multi-objective and convert Solution Vector to Fitness if <see cref="SetScore"/> is not used.</remarks>
        /// <param name="scoreToFit">Delegate converting Score to Fitness.</param>
        public void SetFitness(Func<double[], double> scoreToFit)
        {
            var fitnessValue = Score == null
                ? scoreToFit(SolutionVector)
                : scoreToFit(Score);
            Fitness = fitnessValue;
        }

        /// <summary>
        /// Sets the legality of the individual.
        /// </summary>
        /// <param name="legal">True/false: is/isn't legal</param>
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