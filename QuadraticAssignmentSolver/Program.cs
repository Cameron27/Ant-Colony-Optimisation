using System;
using System.ComponentModel.DataAnnotations;
using Cocona;

namespace QuadraticAssignmentSolver
{
    public class Program
    {
        public enum Algorithm
        {
            Concurrent,
            Replicated,
            Synchronous
        }

        public static bool UseDefaultParameters = true;

        public static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args, options => options.EnableShellCompletionSupport = false);
        }

        [PrimaryCommand]
        public void Run([Argument(Description = "File containing problem.")]
            string file,
            [Option('a', Description = "Algorithm to use")]
            Algorithm algorithm = Algorithm.Concurrent,
            [Option('c', Description = "Number of ants per iteration", ValueName = "1..100")] [Range(1, 100)]
            int antCount = 5,
            [Option('s', Description = "Number of iterations to stop after if there is no improvement",
                ValueName = "1..1000")]
            [Range(1, 1000)]
            int stopThreshold = 20,
            [Option('t',
                Description = "Number of threads to use for multi-threaded algorithms (Default: Total CPU Threads)",
                ValueName = "1..1024")]
            [Range(1, 1024)]
            int? threads = null)
        {
            // Set default processor count
            threads ??= Environment.ProcessorCount;

            // Create search object
            AntColonyOptimiser aco;
            try
            {
                aco = new AntColonyOptimiser(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Solution result;
            // Select algorithm
            switch (algorithm)
            {
                case Algorithm.Concurrent:
                    // Set default parameters
                    if (UseDefaultParameters)
                    {
                        AntColonyOptimiser.FitnessWeight = 1.5;
                        AntColonyOptimiser.PheromoneWeight = 1.5;
                        PheromoneTable.InitialValue = 5;
                        PheromoneTable.EvaporationRate = 0.4;
                    }

                    // Search
                    result = aco.Search(antCount, stopThreshold);
                    break;
                case Algorithm.Replicated:
                    // Set default parameters
                    if (UseDefaultParameters)
                    {
                        AntColonyOptimiser.FitnessWeight = 1.5;
                        AntColonyOptimiser.PheromoneWeight = 1.5;
                        PheromoneTable.InitialValue = 5;
                        PheromoneTable.EvaporationRate = 0.4;
                    }

                    // Search
                    result = aco.ReplicatedParallelSearch(antCount, stopThreshold, (int) threads);
                    break;
                case Algorithm.Synchronous:
                    // Set default parameters
                    if (UseDefaultParameters)
                    {
                        AntColonyOptimiser.FitnessWeight = 1.5;
                        AntColonyOptimiser.PheromoneWeight = 1.5;
                        PheromoneTable.InitialValue = 5;
                        PheromoneTable.EvaporationRate = 0.4;
                    }

                    // Search
                    result = aco.SynchronousParallelSearch(antCount, stopThreshold, (int) threads);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
            }

            result.DisplayResult();
        }
    }
}