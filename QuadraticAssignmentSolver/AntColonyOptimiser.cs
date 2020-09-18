using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        /// <param name="threads">The number of threads to use when generating solutions.</param>
        /// <param name="divisionCount">The number of points to return the best solution at throughout the search.</param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        public Solution[] Search(int antCount, double runtime, int threads, int divisionCount)
        {
            PheromoneTable pheromoneTable = new PheromoneTable(_problem);

            Solution best = null;
            Solution[] bestAtDivisions = new Solution[divisionCount];

            long startTime = DateTime.Now.Ticks;
            long totalAllowedTime = (long) (runtime * TimeSpan.TicksPerSecond);
            long divisionSize = totalAllowedTime / divisionCount;
            int currentDivision = 0;

            // While the stop condition has not been reached
            while (true)
            {
                // Run ants
                IEnumerable<Solution> results = RunAnts(antCount, threads, pheromoneTable);

                // Set new best solution
                if (IsNewBest(results, ref best))
                    results = results.Append(best);

                // Deposit pheromones
                pheromoneTable.DepositPheromones(results);

                // Check if divisions have been reached and results need to be saved and end if final result was saved
                if (StoreSolutions(divisionCount, divisionSize, startTime, bestAtDivisions, best, ref currentDivision))
                    break;
            }

            return bestAtDivisions;
        }

        private IEnumerable<Solution> RunAnts(int antCount, int threads, PheromoneTable pheromoneTable)
        {
            // Run concurrent if thread count it 1
            if (threads == 1)
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
                ParallelFor(0, antCount, threads,
                    _ => results.Add(LocalSearch(ConstructAntSolution(pheromoneTable))));
                return results;
            }
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
        ///     Perform a search of the solution space using the ACO algorithm.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use when generating solutions.</param>
        /// <returns>The best solution found.</returns>
        public Solution Search(int antCount, double runtime, int threads)
        {
            return Search(antCount, runtime, 1, threads)[0];
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
                    double fitness = 1d / solution.PartialFitness(location);

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
        ///     Perform a search of the solution space using the ACO algorithm multiple times in parallel.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <param name="divisionCount">The number of points to return the best solution at throughout the search.</param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        public Solution[] ReplicatedSearch(int antCount, double runtime, int threads, int divisionCount)
        {
            // Spawn a thread for each search and save result
            ConcurrentBag<Solution[]> globalSolutions = new ConcurrentBag<Solution[]>();
            ParallelFor(0, threads, threads, _ =>
            {
                Solution[] solutions = Search(antCount, runtime, 1, divisionCount);
                globalSolutions.Add(solutions);
            });

            // Get best results
            Solution[] bestSolutions = new Solution[divisionCount];
            int[] bestFitnesses = new int[divisionCount];
            Array.Fill(bestFitnesses, int.MaxValue);
            foreach (Solution[] results in globalSolutions)
                for (int i = 0; i < divisionCount; i++)
                {
                    int fitness = results[i].Fitness;

                    if (fitness >= bestFitnesses[i]) continue;

                    bestSolutions[i] = results[i];
                    bestFitnesses[i] = fitness;
                }

            return bestSolutions;
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
            return Search(antCount, runtime, threads, divisionCount);
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
        ///     Perform a search of the solution space using the ACO algorithm multiple times in parallel with information from the
        ///     pheromone tables being shared between each instance of the search.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <param name="divisionCount">
        ///     The number of points to return the best solution at throughout the search.
        /// </param>
        /// <returns>An array of the best solution found at each division, the last solution is the best solution found overall.</returns>
        public Solution[] CourseGrainedSearch(int antCount, double runtime, int threads, int divisionCount)
        {
            // Number of syncs that will be performed between threads
            const int numOfSyncs = 5;

            ConcurrentBag<Solution[]> globalSolutions = new ConcurrentBag<Solution[]>();

            // Create pheromone tables
            PheromoneTable[] pheromoneTables = new PheromoneTable[threads];
            for (int i = 0; i < threads; i++) pheromoneTables[i] = new PheromoneTable(_problem);

            // Create events used for thread synchronisation
            ManualResetEvent[] synchroniseEvents1 = new ManualResetEvent[threads];
            ManualResetEvent[] synchroniseEvents2 = new ManualResetEvent[threads];
            for (int i = 0; i < threads; i++)
            {
                synchroniseEvents1[i] = new ManualResetEvent(false);
                synchroniseEvents2[i] = new ManualResetEvent(false);
            }

            // Create events used to allow threads to continue once they synchronise
            AutoResetEvent gateEvent1 = new AutoResetEvent(false);
            AutoResetEvent gateEvent2 = new AutoResetEvent(false);

            // List of indices of remaining threads used to determine which thread should handle pheromone sharing
            List<int> remainingThreads = Enumerable.Range(0, threads).ToList();

            // Create supervisor thread
            bool terminateSupervisor = false;
            object terminateSupervisorLock = new object();
            Thread supervisor = new Thread(Supervisor);
            supervisor.Start();

            // Run search threads in parallel
            ParallelFor(0, threads, threads, SearchThread);

            // Unset one of synchroniseEvents1 so supervisor will timeout and check for terminateSupervisor being set to
            // true
            synchroniseEvents1[0].Reset();
            lock (terminateSupervisorLock)
            {
                terminateSupervisor = true;
            }

            // Get best results
            Solution[] bestSolutions = new Solution[divisionCount];
            int[] bestFitnesses = new int[divisionCount];
            Array.Fill(bestFitnesses, int.MaxValue);
            foreach (Solution[] results in globalSolutions)
                for (int i = 0; i < divisionCount; i++)
                {
                    int fitness = results[i].Fitness;

                    if (fitness >= bestFitnesses[i]) continue;

                    bestSolutions[i] = results[i];
                    bestFitnesses[i] = fitness;
                }

            return bestSolutions;

            // A method to run as a thread to search the solution space
            void SearchThread(int index)
            {
                Solution best = null;
                Solution[] bestAtDivisions = new Solution[divisionCount];

                long startTime = DateTime.Now.Ticks;
                long totalAllowedTime = (long) (runtime * TimeSpan.TicksPerSecond);
                long divisionSize = totalAllowedTime / divisionCount;
                int currentDivision = 0;
                int iterationsRun = 0;
                int nextPheromoneShareIndex = 1;

                // While the stop condition has not been reached
                while (true)
                {
                    // Run ants
                    IEnumerable<Solution> results = RunAnts(antCount, threads, pheromoneTables[index]);

                    // Set new best solution
                    if (IsNewBest(results, ref best))
                        results = results.Append(best);

                    // Deposit pheromones
                    pheromoneTables[index].DepositPheromones(results);

                    // Check if divisions have been reached and results need to be saved and end if final result was saved
                    if (StoreSolutions(divisionCount, divisionSize, startTime, bestAtDivisions, best,
                        ref currentDivision))
                        break;

                    iterationsRun++;


                    // Sync if enough time has passed
                    if (DateTime.Now.Ticks - startTime <
                        nextPheromoneShareIndex * totalAllowedTime / numOfSyncs) continue;

                    nextPheromoneShareIndex++;

                    // Synchronise all running threads at this point
                    synchroniseEvents1[index].Set();
                    gateEvent1.WaitOne();
                    synchroniseEvents1[index].Reset();

                    // Have one thread share pheromone data
                    if (index == remainingThreads[0])
                    {
                        // New pheromones are the max for each index from the pheromone table
                        double[] newPheromones = new double[_problem.Size * _problem.Size];
                        foreach (PheromoneTable pheromoneTable in pheromoneTables)
                        {
                            double[] pheromones = pheromoneTable.GetAllPheromones();
                            for (int i = 0; i < pheromones.Length; i++) newPheromones[i] += pheromones[i] / threads;
                        }

                        // Assign new pheromones
                        foreach (PheromoneTable pheromoneTable in pheromoneTables)
                            pheromoneTable.SetAllPheromones(newPheromones);
                    }

                    // Synchronise all running threads at this point
                    synchroniseEvents2[index].Set();
                    gateEvent2.WaitOne();
                    synchroniseEvents2[index].Reset();
                }

                // Remove self from and mark as synchronised to allow other threads to continue
                remainingThreads.Remove(index);
                synchroniseEvents1[index].Set();
                synchroniseEvents2[index].Set();

                // Add best result
                globalSolutions.Add(bestAtDivisions);
            }

            // A method to run as a thread that will allow each thread to continue when they all reach the
            // synchronisation points
            void Supervisor()
            {
                while (true)
                    // Wait for all threads to reach synchronisation point 1
                    if (WaitHandle.WaitAll(synchroniseEvents1, 100))
                    {
                        // Stop access through synchronisation point 2
                        gateEvent2.Reset();

                        // Let all the threads through synchronisation point 1
                        for (int i = 0; i < threads; i++) gateEvent1.Set();

                        // Wait for all threads to reach synchronisation point 2
                        WaitHandle.WaitAll(synchroniseEvents2);

                        // Stop access through synchronisation point 1
                        gateEvent1.Reset();

                        // Let all the threads through synchronisation point 2
                        for (int i = 0; i < threads; i++) gateEvent2.Set();
                    }
                    else
                    {
                        // Check if thread should end
                        lock (terminateSupervisorLock)
                        {
                            if (!terminateSupervisor) continue;

                            return;
                        }
                    }
            }
        }

        /// <summary>
        ///     Perform a search of the solution space using the ACO algorithm multiple times in parallel with information from the
        ///     pheromone tables being shared between each instance of the search.
        /// </summary>
        /// <param name="antCount">The number of ants to use in the search.</param>
        /// <param name="runtime">The number of iterations without any improvement to stop after.</param>
        /// <param name="threads">The number of threads to use.</param>
        /// <returns>The best solution found.</returns>
        public Solution CourseGrainedSearch(int antCount, double runtime, int threads)
        {
            return CourseGrainedSearch(antCount, runtime, threads, 1)[0];
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