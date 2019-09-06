using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.Calculation
{
    /// <summary>
    /// Implements the Fast Non-Dominated Sorting algorithm proposed by Deb et al (2002)
    /// </summary>
    public class FastNonDominatedSort : IDominationSorter
    {
        private const string DominationCount = "Domination Count (temporary)";
        
        /// <summary>
        /// Performs sorting. Makes use of:
        /// - <see cref="OptimiserPropertyNames.Dominating"/>
        /// - <see cref="OptimiserPropertyNames.DominatedBy"/>
        /// - <see cref="OptimiserPropertyNames.ParetoFront"/>
        /// </summary>
        /// <param name="individuals">All individuals to consider while calculating Pareto Fronts and domination.</param>
        public void PerformSort(IEnumerable<Individual> individuals)
        {
            var inds = individuals as Individual[] ?? individuals.ToArray();
            
            // First loop: calculate domination
            var currentParetoFront = new List<Individual>();
            
            for (var p = 0; p < inds.Length; p++)
            {
                var individualP = inds.ElementAt(p);
                var dominatedByP = getDominatingList(individualP);
                var dominatingP = getDominatedByList(individualP);
                
                for (var q = 0; q < inds.Count(); q++)
                {
                    if (q == p)
                        continue;
                    
                    var individualQ = inds.ElementAt(q);
                    var dominatedByQ = getDominatingList(individualQ);
                    var dominatingQ = getDominatedByList(individualQ);

                    if (individualP.IsDominating(individualQ))
                    {
                        if (!dominatedByP.Contains(individualQ))
                            dominatedByP.Add(individualQ);
                        if (!dominatingQ.Contains(individualP))
                            dominatingQ.Add(individualP);
                    }
                    else if (individualQ.IsDominating(individualP))
                    {
                        if (!dominatedByQ.Contains(individualP))
                            dominatedByQ.Add(individualP);
                        if (!dominatingP.Contains(individualQ))
                            dominatingP.Add(individualQ);
                    }
                    
                    individualQ.SetProperty(OptimiserPropertyNames.Dominating, dominatedByQ);
                    individualQ.SetProperty(OptimiserPropertyNames.DominatedBy, dominatingQ);
                }
                
                individualP.SetProperty(OptimiserPropertyNames.Dominating, dominatedByP);
                individualP.SetProperty(OptimiserPropertyNames.DominatedBy, dominatingP);
                individualP.SetProperty(DominationCount, dominatingP.Count);

                // If individual P is non-dominated (i.e. in the first Pareto Front), store
                if (dominatingP.Count > 0) 
                    continue;
                
                individualP.SetProperty(OptimiserPropertyNames.ParetoFront, 1);
                currentParetoFront.Add(individualP);
            }
            
            // Second loop: update Pareto Fronts
            var currentFront = 1;
            while (currentParetoFront.Count > 0)
            {
                var nextFrontMembers = new List<Individual>();
                
                foreach (var individualP in currentParetoFront)
                {
                    foreach (var individualQ in individualP
                        .GetProperty<List<Individual>>(OptimiserPropertyNames.Dominating))
                    {
                        var dominationCounter = individualQ.GetProperty<int>(DominationCount) - 1;
                        individualQ.SetProperty(DominationCount, dominationCounter);

                        if (dominationCounter != 0) 
                            continue;
                        
                        individualQ.SetProperty(OptimiserPropertyNames.ParetoFront, currentFront + 1);
                        nextFrontMembers.Add(individualQ);
                    }
                }

                currentFront++;
                currentParetoFront = nextFrontMembers;
            }
        }

        private List<Individual> getDominatingList(Individual individual)
        {
            return individual.GetPropertyNames().Contains(OptimiserPropertyNames.Dominating)
                ? individual.GetProperty<List<Individual>>(OptimiserPropertyNames.Dominating)
                : new List<Individual>();
        }
        
        private List<Individual> getDominatedByList(Individual individual)
        {
            return individual.GetPropertyNames().Contains(OptimiserPropertyNames.DominatedBy)
                ? individual.GetProperty<List<Individual>>(OptimiserPropertyNames.DominatedBy)
                : new List<Individual>();
        }
    }
}