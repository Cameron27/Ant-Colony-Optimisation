using System;
using System.ComponentModel.DataAnnotations;
using Cocona;

namespace QuadraticAssignmentSolver
{
    public class Program
    {
        public static bool UseDefaultParameters = true;

        public static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args, options => options.EnableShellCompletionSupport = false);
        }

        [PrimaryCommand]
        public void Run([Argument(Description = "File containing problem.")]
            string file,
            [Option('a', Description = "Algorithm to use")]
            AntColonyOptimiser.Algorithm algorithm = AntColonyOptimiser.Algorithm.Sequential,
            [Option('c', Description = "Number of ants per iteration", ValueName = "1..100")] [Range(1, 100)]
            int antCount = 5,
            [Option('s', Description = "The length of time to run the search for",
                ValueName = "1..1000")]
            [Range(1, 1000)]
            int runtime = 2,
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
                aco = new AntColonyOptimiser(file, algorithm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            // Select algorithm
            Solution result = algorithm switch
            {
                AntColonyOptimiser.Algorithm.Sequential => aco.SequentialSearch(antCount, runtime).Solution,
                AntColonyOptimiser.Algorithm.Replicated => aco.ReplicatedSearch(antCount, runtime, (int) threads)
                    .Solution,
                AntColonyOptimiser.Algorithm.Synchronous => aco.SynchronousSearch(antCount, runtime, (int) threads)
                    .Solution,
                AntColonyOptimiser.Algorithm.Cooperative => aco.CooperativeSearch(antCount, runtime, 15, (int) threads)
                    .Solution,
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
            };

            result.DisplayResult();
        }
    }
}