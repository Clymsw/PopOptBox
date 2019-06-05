namespace Optimisation.Base.Management
{
    public static partial class OptimiserDefinitions
    {
        /// <summary>
        /// An index to indicate how far through the optimisation process the individual was created.
        /// TODO: move to the Evolutionary Algorithm section
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
        /// TODO: move to the Evolutionary Algorithm section
        /// </summary>
        public const string ReinsertionIndex = "Generation reinserted";
        
        /// <summary>
        /// The real time at which the individual was reinserted into the Population after evaluation.
        /// Set by <see cref="Optimiser"/>.
        /// </summary>
        public const string ReinsertionTime = "Time reinserted";
    }
}