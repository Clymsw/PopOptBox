using System;
using System.Collections.Generic;
using System.Linq;
using PopOptBox.Base.Management;

namespace PopOptBox.Base.MultiObjectiveCalculation
{
    public static class ParetoFrontMetrics
    {
        /// <summary>
        /// Implements the Crowding Distance algorithm proposed by Deb et al. (2002).
        /// Note that the individuals provided should be on the same Pareto Front.
        /// Calculates the crowding distance and assigns it into the individual based on the property name provided.
        /// Extended by consideration of a (nadir) reference point, which can limit the amount of credit an individual at the edge of the Pareto Front can get. 
        /// </summary>
        /// <param name="individuals">Individuals forming a Pareto Front.</param>
        /// <param name="propertyName">Name to give to the property inserted into the <see cref="Individual"/>s.</param>
        /// <param name="referencePoint">The reference point to use (the worst acceptable value, not the best) for calculating crowding distance for extremal values.</param>
        public static void AssignCrowdingDistance(IEnumerable<Individual> individuals, string propertyName,
            IEnumerable<bool> minimise, IEnumerable<double> referencePoint)
        {
            var inds = individuals as Individual[] ?? individuals.ToArray();
            var isMinimiser = minimise.ToArray();
            var reference = referencePoint.ToArray();
            var propertyOutOfBounds = "[Crowding Distance calculation] Out of bounds";
            
            for (var m = 0; m < inds[0].SolutionVector.Length; m++)
            {
                // Sort individuals worst->best by this objective
                Individual[] tempSorted;
                var fmin = reference.ElementAt(m);
                var fmax = reference.ElementAt(m);
                if (isMinimiser.ElementAt(m))
                {
                    tempSorted = inds.OrderByDescending(a => a.SolutionVector.ElementAt(m)).ToArray();
                    if (fmax > tempSorted.First().SolutionVector.ElementAt(m))
                    {
                        fmax = tempSorted.First().SolutionVector.ElementAt(m);
                    }
                    fmin = tempSorted.Last().SolutionVector.ElementAt(m);
                }
                else
                {
                    tempSorted = inds.OrderBy(a => a.SolutionVector.ElementAt(m)).ToArray();
                    if (fmin < tempSorted.First().SolutionVector.ElementAt(m))
                    {
                        fmin = tempSorted.First().SolutionVector.ElementAt(m);
                    }
                    fmax = tempSorted.Last().SolutionVector.ElementAt(m);
                }

                // Loop through each individual except the last and assign crowding distance.
                var worseThanReference = true;
                for (var i = 0; i < inds.Length - 1; i++)
                {
                    try
                    {
                        var oob = tempSorted.ElementAt(i).GetProperty<bool>(propertyOutOfBounds);
                        // If it's set, it's true
                        // We ignore this individual on every objective if it is terrible on one.
                        continue;
                    } catch {}
                    
                    var distance = 0.0;
                    if (m > 0)
                        distance = tempSorted.ElementAt(i).GetProperty<double>(propertyName);

                    var currentSolution = tempSorted.ElementAt(i).SolutionVector.ElementAt(m);

                    // Check to see if we are better than the reference point. 
                    if (worseThanReference)
                    {
                        if (isMinimiser.ElementAt(m))
                        {
                            if (currentSolution > reference.ElementAt(m))
                            {
                                // Still worse, assign zero crowding distance
                                tempSorted.ElementAt(i).SetProperty(propertyName, 0.0);
                                // Mark at as out of bounds so we don´t include it later
                                tempSorted.ElementAt(i).SetProperty(propertyOutOfBounds, true);
                                continue;
                            }

                            tempSorted.ElementAt(i).SetProperty(propertyName,
                                reference.ElementAt(m) == double.MaxValue
                                    ? double.MaxValue
                                    : distance + (
                                          reference.ElementAt(m) -
                                          tempSorted.ElementAt(i + 1).SolutionVector.ElementAt(m)) / 
                                      (fmax - fmin));
                        }
                        else
                        {
                            if (currentSolution < reference.ElementAt(m))
                            {
                                // Still worse, assign zero crowding distance
                                tempSorted.ElementAt(i).SetProperty(propertyName, 0.0);
                                // Mark at as out of bounds so we don´t include it later
                                tempSorted.ElementAt(i).SetProperty(propertyOutOfBounds, true);
                                continue;
                            }
                            
                            tempSorted.ElementAt(i).SetProperty(propertyName,
                                reference.ElementAt(m) == double.MinValue
                                    ? double.MaxValue
                                    : distance + (
                                          tempSorted.ElementAt(i + 1).SolutionVector.ElementAt(m) -
                                          reference.ElementAt(m)) /
                                      (fmax - fmin));
                        }

                        worseThanReference = false;
                    }
                    else
                    {
                        if (isMinimiser.ElementAt(m))
                        {
                            tempSorted.ElementAt(i).SetProperty(propertyName,
                                distance + 
                                (tempSorted.ElementAt(i - 1).SolutionVector.ElementAt(m) -
                                 tempSorted.ElementAt(i + 1).SolutionVector.ElementAt(m)) /
                                (fmax - fmin));
                        }
                        else
                        {
                            tempSorted.ElementAt(i).SetProperty(propertyName,
                                distance + 
                                (tempSorted.ElementAt(i + 1).SolutionVector.ElementAt(m) -
                                 tempSorted.ElementAt(i - 1).SolutionVector.ElementAt(m)) /
                                (fmax - fmin));
                        }
                    }
                }

                // Assign max crowding distance for the last one.
                tempSorted.Last().SetProperty(propertyName, double.MaxValue);
            }
        }
    }
}