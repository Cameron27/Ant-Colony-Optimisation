using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuadraticAssignmentSolver
{
    public class AntColonyOptimiser
    {
        public static double FitnessWeight = 1;
        public static double PheromoneWeight = 1;

        private readonly Problem _problem;

        public AntColonyOptimiser(string filename)
        {
            _problem = Problem.CreateFromFile(filename);
        }

        public (Solution, int) Search(int antCount, double stopThreshold)
        {
            PheromoneTable pheromoneTable = new PheromoneTable(_problem);

            (Solution, int) best = (null, int.MaxValue);

            int iterationsNoImprovement = 0;
            while (iterationsNoImprovement < stopThreshold)
            {
                List<(Solution, int)> results = new List<(Solution, int)>();
                for (int j = 0; j < antCount; j++) results.Add(LocalSearch(ConstructAntSolution(pheromoneTable)));

                (Solution, int) oldBest = best;

                foreach ((Solution, int) result in results)
                    if (result.Item2 < best.Item2)
                        best = result;

                if (oldBest.Item2 == best.Item2)
                {
                    results.Add(best);
                    iterationsNoImprovement++;
                }
                else
                {
                    iterationsNoImprovement = 0;
                }

                pheromoneTable.DepositPheromones(results);
            }

            return best;
        }

        private (Solution, int) ConstructAntSolution(PheromoneTable pheromoneTable)
        {
            Random rnd = new Random();

            List<int> remainingLocations = Enumerable.Range(0, _problem.Size).ToList();
            List<int> remainingFacilities = Enumerable.Range(0, _problem.Size).ToList();

            Solution solution = new Solution(_problem);

            while (remainingLocations.Count != 0)
            {
                int location = remainingLocations[rnd.Next(remainingLocations.Count)];

                double[] weightings = new double[remainingFacilities.Count];

                for (int i = 0; i < remainingFacilities.Count; i++)
                {
                    int facility = remainingFacilities[i];

                    solution.SetFacility(location, facility);

                    double fitness = 1d / solution.EvaluatePartialFitness(location);
                    double pheromone = pheromoneTable.GetPheromones(location, facility);

                    weightings[i] = Math.Pow(fitness, FitnessWeight) * Math.Pow(pheromone, PheromoneWeight);
                }

                double weightingSum = weightings.Sum();
                double r = rnd.NextDouble() * weightingSum;
                int index;
                for (index = 0; index < weightings.Length; index++)
                {
                    r -= weightings[index];
                    if (r <= 0) break;
                }

                if (index == remainingFacilities.Count) index--;

                solution.SetFacility(location, remainingFacilities[index]);

                remainingLocations.Remove(location);
                remainingFacilities.RemoveAt(index);
            }

            return (solution, solution.EvaluateFitness());
        }

        private (Solution, int) LocalSearch((Solution, int) start)
        {
            Solution startSolution = start.Item1;

            Solution solution = startSolution.Clone();

            while (true)
            {
                int bestFitnessDiff = int.MaxValue;
                (int, int) bestSwap = (0, 0);
                for (int locationA = 0; locationA < solution.Size; locationA++)
                {
                    int facilityA = solution.GetFacility(locationA);
                    int partialFitnessA = solution.EvaluatePartialFitness(locationA);

                    solution.SetFacility(locationA, -1);

                    for (int locationB = locationA + 1; locationB < solution.Size; locationB++)
                    {
                        int facilityB = solution.GetFacility(locationB);

                        int partialFitnessB = solution.EvaluatePartialFitness(locationB);

                        solution.SetFacility(locationB, facilityA);

                        int partialFitnessC = solution.EvaluatePartialFitness(locationB);

                        solution.SetFacility(locationA, facilityB);

                        int partialFitnessD = solution.EvaluatePartialFitness(locationA);

                        int fitnessDiff = -partialFitnessA - partialFitnessB + partialFitnessD + partialFitnessC;

                        if (fitnessDiff < bestFitnessDiff)
                        {
                            bestFitnessDiff = fitnessDiff;
                            bestSwap = (locationA, locationB);
                        }

                        solution.SetFacility(locationA, -1);
                        solution.SetFacility(locationB, facilityB);
                    }

                    solution.SetFacility(locationA, facilityA);
                }

                if (bestFitnessDiff >= 0) break;

                (int lA, int lB) = bestSwap;
                (int fA, int fB) = (solution.GetFacility(lA), solution.GetFacility(lB));
                solution.SetFacility(lA, fB);
                solution.SetFacility(lB, fA);
            }

            return (solution, solution.EvaluateFitness());
        }

        public (Solution, int) SynchronousParallelSearch(int antCount, double stopThreshold, int threads)
        {
            ConcurrentBag<(Solution, int)> results = new ConcurrentBag<(Solution, int)>();

            Parallel.For(0, threads, _ =>
            {
                (Solution, int) result = Search(antCount, stopThreshold);
                results.Add(result);
            });

            (Solution, int) bestResult = (null, int.MaxValue);
            foreach ((Solution, int) result in results)
                if (result.Item2 < bestResult.Item2)
                    bestResult = result;

            return bestResult;
        }
    }
}