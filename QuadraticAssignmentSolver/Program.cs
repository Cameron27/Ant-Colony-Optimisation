using System;
using System.ComponentModel.DataAnnotations;
using Cocona;

namespace QuadraticAssignmentSolver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CoconaLiteApp.Run<Program>(args, options => options.EnableShellCompletionSupport = false);
        }

        [PrimaryCommand]
        public void Run([Argument(Description = "File containing problem.")]
            string file, [Option('c', Description = "Number of ants per iteration", ValueName = "1..100")]
            [Range(1, 100)]
            int antCount = 5,
            [Option('s', Description = "Number of iterations to stop after if there is no improvement",
                ValueName = "1..1000")]
            [Range(1, 1000)]
            int stopThreshold = 10)
        {
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

            (Solution, int) result = aco.Search(antCount, stopThreshold);
            result.DisplayResult();
        }
    }
}