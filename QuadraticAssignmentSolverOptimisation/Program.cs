using System;
using QuadraticAssignmentSolver.Optimisation;

namespace QuadraticAssignmentSolverOptimisation
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("No argument provided.");
                return;
            }

            switch (args[0].ToLower())
            {
                case "concurrent":
                    new ConcurrentOptimisation().Run();
                    break;
                case "synchronous":
                    new SynchronousParallelOptimisation().Run();
                    break;
                case "count_threshold":
                    new CountThresholdOptimisation().Run();
                    break;
                default:
                    Console.Error.WriteLine("Argument is invalid.");
                    break;
            }
        }
    }
}