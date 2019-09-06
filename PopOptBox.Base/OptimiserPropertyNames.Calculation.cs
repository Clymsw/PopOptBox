namespace PopOptBox.Base
{
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