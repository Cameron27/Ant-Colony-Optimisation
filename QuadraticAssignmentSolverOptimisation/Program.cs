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
                case "concurrent_performance":
                    new ConcurrentPerformanceTest().Run();
                    break;
                case "replicated_performance":
                    new ReplicatedPerformanceTest().Run();
                    break;
                case "synchronous_performance":
                    new SynchronousPerformanceTest().Run();
                    break;
                case "course_grained_performance":
                    new CourseGrainedPerformanceTest().Run();
                    break;
                case "all_performance":
                    new AllPerformanceTest().Run();
                    break;
                default:
                    Console.Error.WriteLine("Argument is invalid.");
                    break;
            }
        }
    }
}