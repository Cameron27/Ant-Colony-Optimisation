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

        public Solution Search(int antCount, double stopThreshold)
        {
            PheromoneTable pheromoneTable = new PheromoneTable(_problem);

            Solution best = null;

            // While the stop condition has not been reached
            int iterationsNoImprovement = 0;
            while (iterationsNoImprovement < stopThreshold)
            {
                // Generate solutions with ants and local search
                List<Solution> results = new List<Solution>();
                for (int j = 0; j < antCount; j++) results.Add(LocalSearch(ConstructAntSolution(pheromoneTable)));

                Solution oldBest = best;

                // Set new best solution
                foreach (Solution result in results.Where(result =>
                    best == null || result.Fitness < best.Fitness))
                    best = result;

                // Add best solution to the results if it is not already in there
                if (oldBest != null && oldBest.Fitness == best.Fitness)
                {
                    results.Add(best);
                    iterationsNoImprovement++;
                }
                else
                {
                    iterationsNoImprovement = 0;
                }

                // Deposit pheromones
                pheromoneTable.DepositPheromones(results);
            }

            return best;
        }

        private Solution ConstructAntSolution(PheromoneTable pheromoneTable)
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

                    double fitness = 1d / solution.PartialFitness(location);
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

            return solution;
        }

        private Solution LocalSearch(Solution solution)
        {
            solution = solution.Clone();

            while (true)
            {
                int bestFitnessDiff = int.MaxValue;
                (int, int) bestSwap = (0, 0);
                for (int locationA = 0; locationA < solution.Size; locationA++)
                {
                    int facilityA = solution.GetFacility(locationA);
                    int partialFitnessA = solution.PartialFitness(locationA);

                    solution.SetFacility(locationA, null);

                    for (int locationB = locationA + 1; locationB < solution.Size; locationB++)
                    {
                        int facilityB = solution.GetFacility(locationB);

                        int partialFitnessB = solution.PartialFitness(locationB);

                        solution.SetFacility(locationB, facilityA);

                        int partialFitnessC = solution.PartialFitness(locationB);

                        solution.SetFacility(locationA, facilityB);

                        int partialFitnessD = solution.PartialFitness(locationA);

                        int fitnessDiff = -partialFitnessA - partialFitnessB + partialFitnessD + partialFitnessC;

                        if (fitnessDiff < bestFitnessDiff)
                        {
                            bestFitnessDiff = fitnessDiff;
                            bestSwap = (locationA, locationB);
                        }

                        solution.SetFacility(locationA, null);
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

            return solution;
        }

        public Solution SynchronousParallelSearch(int antCount, double stopThreshold, int threads)
        {
            ConcurrentBag<Solution> results = new ConcurrentBag<Solution>();

            Parallel.For(0, threads, _ =>
            {
                Solution result = Search(antCount, stopThreshold);
                results.Add(result);
            });

            Solution bestResult = null;
            int bestFitness = int.MaxValue;
            foreach (Solution result in results)
            {
                int fitness = result.Fitness;

                if (fitness >= bestFitness) continue;

                bestResult = result;
                bestFitness = fitness;
            }

            return bestResult;
        }
    }
}