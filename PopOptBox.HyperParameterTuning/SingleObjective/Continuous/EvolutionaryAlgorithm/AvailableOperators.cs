namespace PopOptBox.HyperParameterTuning.SingleObjective.Continuous.EvolutionaryAlgorithm
{
    public static class AvailableOperators
    {
        public enum ParentSelector
        {
            Greedy,
            Random,
            Roulette,
            Tournament
        }

        public enum RecombinationOperator
        {
            ArithmeticMultiParent,
            ArithmeticTwoParentWeighted,
            MultiPoint,
            Uniform,
            Sbx,
            Pcx
        }

        public enum MutationOperators
        {
            AddRandomNumber,
            RandomSwap,
            ReplaceWithRandomNumber,
            None
        }

        public enum ReinsertionOperators
        {
            ReplaceWorst,
            ReplaceRandom
        }
    }
}