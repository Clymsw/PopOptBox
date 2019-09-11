using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using PopOptBox.Base.Conversion;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Runtime
{
    /// <summary>
    /// The Evaluation Agent receives un-evaluated individuals and outputs evaluated ones.
    /// </summary>
    internal class EvaluationAgent
    {
        /// <summary>
        /// The block that handles incoming individuals and processes the evaluation.
        /// </summary>
        public TransformBlock<Individual, Individual> IndividualsForEvaluation { get; }

        /// <summary>
        /// The block that buffers evaluated individuals until they are ready for reinsertion.
        /// </summary>
        public BufferBlock<Individual> EvaluatedIndividuals { get; }

        private readonly IEvaluator evaluator;

        private int numberEvaluated;

        /// <summary>
        /// The number of <see cref="Individual"/>s evaluated so far.
        /// </summary>
        public int NumberEvaluated => numberEvaluated;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="cancelToken">Token (from <see cref="ReinsertionAgent"/>) for cancellation.</param>
        public EvaluationAgent(IEvaluator evaluator,
            CancellationToken cancelToken)
        {
            this.evaluator = evaluator;

            numberEvaluated = 0;

            IndividualsForEvaluation = new TransformBlock<Individual, Individual>(
                (Func<Individual, Individual>)Process,
                new ExecutionDataflowBlockOptions
                {
                    CancellationToken = cancelToken,
                    MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded
                });

            EvaluatedIndividuals = new BufferBlock<Individual>(
                new DataflowBlockOptions
                {
                    CancellationToken = cancelToken
                });

            // Setup link so that evaluated individuals are pushed to the output buffer.
            IndividualsForEvaluation.LinkTo(EvaluatedIndividuals);
        }

        /// <summary>
        /// Performs the evaluation.
        /// </summary>
        /// <param name="ind">Unevaluated individual.</param>
        /// <returns>Same individual (after evaluation).</returns>
        private Individual Process(Individual ind)
        {
            evaluator.Evaluate(ind);
            Interlocked.Increment(ref numberEvaluated);

            return ind;
        }
    }
}