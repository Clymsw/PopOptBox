namespace PopOptBox.Base.Management
{
    /// <summary>
    /// The list of names for properties which are automatically created in an <see cref="Individual"/>.
    /// </summary>
    public static class OptimiserPropertyNames
    {
        /// <summary>
        /// An index to indicate how far through the optimisation process the individual was created.
        /// For evolutionary type algorithms, it would be the generation.
        /// </summary>
        public const string CreationIndex = "Generation created";
        
        /// <summary>
        /// The real time at which the individual was created.
        /// Set by <see cref="Optimiser"/>.
        /// </summary>
        public const string CreationTime = "Time created";
        
        /// <summary>
        /// An index to indicate how far through the optimisation process the individual was reinserted into the Population.
        /// </summary>
        public const string ReinsertionIndex = "Generation reinserted";
        
        /// <summary>
        /// The real time at which the individual was reinserted into the Population after evaluation.
        /// Set by <see cref="Optimiser"/>.
        /// </summary>
        public const string ReinsertionTime = "Time reinserted";

        /// <summary>
        /// If an error has been encountered on reinsertion, it is stored here.
        /// </summary>
        public const string ReinsertionError = "Reinsertion error";

        public const string ParetoFront = "Pareto Front";

        public const string DominationRank = "Number of individuals dominating this";

        public const string Dominating = "Individuals dominated";

        public const string DominatedBy = "Individuals dominating this";
    }
}