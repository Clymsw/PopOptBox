namespace PopOptBox.Base
{
    /// <summary>
    /// The list of names for properties which are automatically created in an <see cref="PopOptBox.Base.Management.Individual"/>
    /// during the optimisation.
    /// </summary>
    public static partial class OptimiserPropertyNames
    {
        /// <summary>
        /// The Front which the <see cref="PopOptBox.Base.Management.Individual"/> is currently on.
        /// Indexed to 1 by convention.
        /// </summary>
        public const string ParetoFront = "Pareto Front";

        /// <summary>
        /// The list of <see cref="PopOptBox.Base.Management.Individual"/>s which are dominated by this one.
        /// </summary>
        public const string Dominating = "Individuals dominated";

        /// <summary>
        /// The list of <see cref="PopOptBox.Base.Management.Individual"/>s which dominate this one.
        /// </summary>
        public const string DominatedBy = "Individuals dominating this";
    }
}