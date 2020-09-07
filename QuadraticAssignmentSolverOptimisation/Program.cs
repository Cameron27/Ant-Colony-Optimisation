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
                Console.WriteLine("No argument provided.");
                return;
            }

            switch (args[0].ToLower())
            {
                case "concurrent":
                    new ConcurrentOptimisation().Run();
                    break;
                default:
                    Console.WriteLine("Argument is invalid.");
                    break;
            }
        }
    }
}