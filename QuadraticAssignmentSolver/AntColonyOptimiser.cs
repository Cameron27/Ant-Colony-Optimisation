using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuadraticAssignmentSolver
{
    public class AntColonyOptimiser
    {
        /// <summary>
        ///     The exponent for contribution of fitness.
        /// </summary>
        public static double FitnessWeight = 1;

        /// <summary>
        ///     The exponent for contribution of pheromone.
        /// </summary>
        public static double PheromoneWeight = 1;

        /// <summary>
        ///     The frequency with which to use the the global best solution to deposit pheromones.
        /// </summary>
        public static int GlobalBestDepositFreq = 10;

        /// <summary>
        ///     A random object to be used when a source of randomness is needed.
        /// </summary>
        [ThreadStatic] private static Random _rnd;

        /// <summary>
        ///     The problem being solved.
        /// </summary>
        private readonly Problem _problem;

        /// <summary>
        ///     Initialise a new instance of the <code>AntColonyOptimiser</code> class for a specific problem.
        /// </summary>
        /// <param name="filename">The file containing the problem to be solved.</param>
        public AntColonyOptimiser(string filename)
        {
            _problem = Problem.CreateFromFile(filename);
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="antThreads">The number of threads to use when generating solutions.</param>
        /// <param name="replicatedThreads">The number of threads to use to run multiple copies of the search.</param>
        /// <param name="divisionCount">The number of points to return the best solution at throughout the search.</param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        private Solution[] Search(int antCount, double runtime, int divisionCount, int antThreads = 1,
            int replicatedThreads = 1)
        {
            // Get start time
            long startTime = DateTime.Now.Ticks;

            // Create pheromone tables
            PheromoneTable[] pheromoneTables = new PheromoneTable[replicatedThreads];
            for (int i = 0; i < pheromoneTables.Length; i++) pheromoneTables[i] = new PheromoneTable(_problem);

            // Calculate some values
            long totalAllowedTime = (long) (runtime * TimeSpan.TicksPerSecond);
            long divisionSize = totalAllowedTime / divisionCount;

            // Start searches
            Solution[][] globalSolutions = new Solution[replicatedThreads][];
            if (replicatedThreads == 1)
                globalSolutions[0] = SingleSearch(pheromoneTables[0]);
            else
                Parallel.For(0, replicatedThreads, new ParallelOptions {MaxDegreeOfParallelism = replicatedThreads},
                    index => globalSolutions[index] = SingleSearch(pheromoneTables[index]));

            return GetBestSolutions(globalSolutions);

            // A single instance of the search process
            Solution[] SingleSearch(PheromoneTable pheromoneTable)
            {
                Solution best = null;
                Solution[] bestAtDivisions = new Solution[divisionCount];
                int currentDivision = 0;

                // While the stop condition has not been reached
                int iteration = 0;
                while (true)
                {
                    // Run ants
                    Solution[] results = RunAnts(antCount, antThreads, pheromoneTable).ToArray();

                    // Set new best solution
                    if (IsNewBest(results, ref best))
                        pheromoneTable.UpdateMaxAndMin(best);


                    // Deposit pheromones
                    // Decide whether to deposit iteration best or global best solution
                    if (iteration % GlobalBestDepositFreq != 0)
                    {
                        Solution iterationBest = results.OrderBy(r => r.Fitness).First();
                        pheromoneTable.DepositPheromones(iterationBest);
                    }
                    else
                    {
                        pheromoneTable.DepositPheromones(best);
                    }

                    // Check if divisions have been reached and results need to be saved and end if final result was saved
                    if (StoreSolutions(divisionCount, divisionSize, startTime, bestAtDivisions, best,
                        ref currentDivision))
                        break;

                    iteration++;
                }

                return bestAtDivisions;
            }
        }

        /// <summary>
        ///     Run a number of ants to generate solutions.
        /// </summary>
        /// <param name="antCount">The number of ants to run.</param>
        /// <param name="antThreads">The number of threads to use to run ants.</param>
        /// <param name="pheromoneTable">The pheromone table to be used by ants.</param>
        /// <returns>All the solutions generated by the ants.</returns>
        private IEnumerable<Solution> RunAnts(int antCount, int antThreads, PheromoneTable pheromoneTable)
        {
            // Run concurrent if thread count it 1
            if (antThreads == 1)
            {
                // Generate solutions with ants and local search
                List<Solution> results = new List<Solution>();
                for (int j = 0; j < antCount; j++) results.Add(LocalSearch(ConstructAntSolution(pheromoneTable)));
                return results;
            }
            else
            {
                // Generate solutions with ants and local search in parallel
                ConcurrentBag<Solution> results = new ConcurrentBag<Solution>();
                Parallel.For(0, antCount, new ParallelOptions {MaxDegreeOfParallelism = antCount},
                    _ => results.Add(LocalSearch(ConstructAntSolution(pheromoneTable))));
                return results;
            }
        }

        /// <summary>
        ///     Construct a solution with an ant.
        /// </summary>
        /// <param name="pheromoneTable">The pheromone table to use in the construction.</param>
        /// <returns>The solution generated by the ant.</returns>
        private Solution ConstructAntSolution(PheromoneTable pheromoneTable)
        {
            _rnd ??= new Random();

            List<int> remainingLocations = Enumerable.Range(0, _problem.Size).ToList();
            List<int> remainingFacilities = Enumerable.Range(0, _problem.Size).ToList();

            Solution solution = new Solution(_problem);

            // While there are unfilled locations
            while (remainingLocations.Count != 0)
            {
                // Select a random location
                int location = remainingLocations[_rnd.Next(remainingLocations.Count)];

                double[] weightings = new double[remainingFacilities.Count];

                // Calculate weighting for each facility
                for (int i = 0; i < remainingFacilities.Count; i++)
                {
                    int facility = remainingFacilities[i];

                    // Calculate the fitness diff for each facility if it were to be inserted at the location
                    solution.SetFacility(location, facility);
                    double fitness = 1d / (solution.PartialFitness(location) + 1);

                    // Get pheromone for facility at that location
                    double pheromone = pheromoneTable.GetPheromones(location, facility);

                    // Calculate weighting
                    weightings[i] = Math.Pow(fitness, FitnessWeight) * Math.Pow(pheromone, PheromoneWeight);
                }

                // Select a facility with weightings as probabilities
                double weightingSum = weightings.Sum();
                double r = _rnd.NextDouble() * weightingSum;
                int index;
                for (index = 0; index < weightings.Length - 1; index++)
                {
                    r -= weightings[index];
                    if (r <= 0) break;
                }

                // Set facility to chosen facility
                solution.SetFacility(location, remainingFacilities[index]);

                // Update remaining
                remainingLocations.Remove(location);
                remainingFacilities.RemoveAt(index);
            }

            return solution;
        }

        /// <summary>
        ///     Perform a local search on a solution.
        /// </summary>
        /// <param name="solution">The solution to perform the local search on.</param>
        /// <returns>The solution found by local search.</returns>
        public static Solution LocalSearch(Solution solution)
        {
            solution = solution.Clone();

            // Calculate all partial fitnesses
            int[] partialFitnesses = solution.AllPartialFitnesses();

            // Iterate local search
            while (true)
            {
                // Want to find best facility swap to minimise fitness
                int bestFitnessDiff = int.MaxValue;
                (int LocationA, int LocationB) bestSwap = (0, 0);

                // For each facility
                for (int locationA = 0; locationA < solution.Size; locationA++)
                {
                    int facilityA = solution.GetFacility(locationA);

                    // Get partial fitness for that facility
                    int partialFitnessA = partialFitnesses[locationA];

                    // For each other facility
                    for (int locationB = locationA + 1; locationB < solution.Size; locationB++)
                    {
                        int facilityB = solution.GetFacility(locationB);

                        // Get partial fitness for that facility
                        int partialFitnessB = partialFitnesses[locationB];

                        // Note: partialFitnessA and partialFitnessB together double count the AB link but that is later
                        // canceled out by partialFitnessC and partialFitnessD also double counting it

                        // Swap facilities
                        solution.SetFacility(locationB, facilityA);
                        solution.SetFacility(locationA, facilityB);

                        int partialFitnessC = solution.PartialFitness(locationB);
                        int partialFitnessD = solution.PartialFitness(locationA);

                        // Calculate fitness diff and check if it is better
                        int fitnessDiff = -partialFitnessA - partialFitnessB + partialFitnessD + partialFitnessC;
                        if (fitnessDiff < bestFitnessDiff)
                        {
                            bestFitnessDiff = fitnessDiff;
                            bestSwap = (locationA, locationB);
                        }

                        // Reset location B
                        solution.SetFacility(locationA, facilityA);
                        solution.SetFacility(locationB, facilityB);
                    }

                    // Reset location A
                    solution.SetFacility(locationA, facilityA);
                }

                // If no improvement was found, end local search
                if (bestFitnessDiff >= 0) break;

                {
                    // Extract values
                    (int locationA, int locationB) = bestSwap;
                    (int facilityA, int facilityB) = (solution.GetFacility(locationA), solution.GetFacility(locationB));

                    // Update partial fitnesses by removing location A and B
                    for (int location = 0; location < partialFitnesses.Length; location++)
                    {
                        if (location == locationA || location == locationB) continue;
                        partialFitnesses[location] -= solution.PartialFitness(location, locationA);
                        partialFitnesses[location] -= solution.PartialFitness(location, locationB);
                    }

                    // Make swap
                    solution.SetFacility(locationA, facilityB);
                    solution.SetFacility(locationB, facilityA);

                    // Update partial fitnesses by adding new location A and B
                    for (int location = 0; location < partialFitnesses.Length; location++)
                    {
                        if (location == locationA || location == locationB) continue;
                        partialFitnesses[location] += solution.PartialFitness(location, locationA);
                        partialFitnesses[location] += solution.PartialFitness(location, locationB);
                    }

                    // Recalculate partial fitness for location A and B
                    partialFitnesses[locationA] = solution.PartialFitness(locationA);
                    partialFitnesses[locationB] = solution.PartialFitness(locationB);
                }
            }

            return solution;
        }


        /// <summary>
        ///     Checks if there is a new best result in a collection of results.
        /// </summary>
        /// <param name="results">The collection of results to search for a new best result in.</param>
        /// <param name="best">
        ///     A reference to the current best result. If a new best result was found this reference will be set to
        ///     it.
        /// </param>
        /// <returns>True if a new best result was found.</returns>
        private static bool IsNewBest(IEnumerable<Solution> results, ref Solution best)
        {
            Solution oldBest = best;

            // Find if there is a new best
            foreach (Solution result in results)
                if (best == null || result.Fitness < best.Fitness)
                    best = result;

            // Return true if new best was found
            return best != oldBest;
        }

        /// <summary>
        ///     Stores the current result if a division has been reached and then returns if all divisions have been
        ///     reached.
        /// </summary>
        /// <param name="divisionCount">The number of divisions.</param>
        /// <param name="divisionSize">The size of a single division in ticks.</param>
        /// <param name="startTime">The start time of search in ticks.</param>
        /// <param name="bestAtDivisions">A list of best solutions at each division. This list can be modified.</param>
        /// <param name="best">The current best solution.</param>
        /// <param name="currentDivision">A reference to the current division.</param>
        /// <returns>True if the final division was filled.</returns>
        private static bool StoreSolutions(int divisionCount, long divisionSize, long startTime,
            IList<Solution> bestAtDivisions, Solution best, ref int currentDivision)
        {
            long nowTime = DateTime.Now.Ticks;
            // Fill divisions while needed
            while ((currentDivision + 1) * divisionSize < nowTime - startTime)
            {
                // Save current best
                bestAtDivisions[currentDivision] = best;
                currentDivision++;
                // Check if done
                if (currentDivision == divisionCount) return true;
            }

            return false;
        }

        /// <summary>
        ///     Extract the best solution at each division for all the solutions produced.
        /// </summary>
        /// <param name="globalSolutions">A list containing one or more arrays so solutions.</param>
        /// <returns>An array containing the best solution at each division.</returns>
        private static Solution[] GetBestSolutions(IReadOnlyList<Solution[]> globalSolutions)
        {
            // If there is only one set of solutions, just return it
            if (globalSolutions.Count == 1) return globalSolutions[0];

            int divisionCount = globalSolutions[0].Length;

            // The current best solutions at each division
            Solution[] bestSolutions = new Solution[divisionCount];

            // The current best fittnesses at each division
            int[] bestFitnesses = new int[divisionCount];
            Array.Fill(bestFitnesses, int.MaxValue);

            // For each set of solutions
            foreach (Solution[] results in globalSolutions)
                // For each division
                for (int i = 0; i < divisionCount; i++)
                {
                    int fitness = results[i].Fitness;

                    // Check if solution is the new best for that division
                    if (fitness >= bestFitnesses[i]) continue;

                    bestSolutions[i] = results[i];
                    bestFitnesses[i] = fitness;
                }

            return bestSolutions;
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="divisionCount">The number of points to return the best solution at throughout the search.</param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        public Solution[] ConcurrentSearch(int antCount, double runtime, int divisionCount)
        {
            return Search(antCount, runtime, divisionCount);
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <returns>The best solution found.</returns>
        public Solution ConcurrentSearch(int antCount, double runtime)
        {
            return ConcurrentSearch(antCount, runtime, 1)[0];
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm multiple times in parallel.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <param name="divisionCount">The number of points to return the best solution at throughout the search.</param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        public Solution[] ReplicatedSearch(int antCount, double runtime, int threads, int divisionCount)
        {
            return Search(antCount, runtime, divisionCount, replicatedThreads: threads);
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm multiple times in parallel.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <returns>The best solution found.</returns>
        public Solution ReplicatedSearch(int antCount, double runtime, int threads)
        {
            return ReplicatedSearch(antCount, runtime, threads, 1)[0];
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm with solutions generated in parallel.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <param name="divisionCount">The number of points to return the best solution at throughout the search.</param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        public Solution[] SynchronousSearch(int antCount, double runtime, int threads, int divisionCount)
        {
            return Search(antCount, runtime, divisionCount, threads);
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm with solutions generated in parallel.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <returns>The best solution found.</returns>
        public Solution SynchronousSearch(int antCount, double runtime, int threads)
        {
            return SynchronousSearch(antCount, runtime, threads, 1)[0];
        }

        /// <summary>
        ///     Run and action a specified number of times in parallel.
        /// </summary>
        /// <param name="fromInclusive">The index to start from (inclusive).</param>
        /// <param name="toExclusive">The index to stop at (exclusive).</param>
        /// <param name="maxDegreeOfParallelism">The maximum number of threads to run at a time.</param>
        /// <param name="action">The action to run. Each call of the action with have an index passed into it.</param>
        private static void ParallelFor(int fromInclusive, int toExclusive, int maxDegreeOfParallelism,
            Action<int> action)
        {
            int count = toExclusive - fromInclusive;
            // Queue of indices to run
            ConcurrentQueue<int> indices = new ConcurrentQueue<int>(Enumerable.Range(fromInclusive, count));

            // Create and run threads then wait for them to finish
            Enumerable.Range(0, Math.Min(count, maxDegreeOfParallelism))
                .Select(_ =>
                {
                    Thread t = new Thread(MainThread);
                    t.Start();
                    return t;
                })
                .ToList()
                .ForEach(thread => thread.Join());

            // Method that makes the thread that is run
            void MainThread()
            {
                // While there are still remaining indices, take one and run it
                while (indices.TryDequeue(out int index))
                {
                    int i = index;
                    action.Invoke(i);
                }
            }
        }
    }
}