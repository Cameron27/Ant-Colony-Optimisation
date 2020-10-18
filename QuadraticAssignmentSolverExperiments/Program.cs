using System;

namespace QuadraticAssignmentSolver.Experiments
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
                case "sequential_performance":
                    new SequentialPerformanceTest().Run();
                    break;
                case "replicated_performance":
                    new ReplicatedPerformanceTest().Run();
                    break;
                case "synchronous_performance":
                    new SynchronousPerformanceTest().Run();
                    break;
                case "cooperative_performance":
                    new CooperativePerformanceTest().Run();
                    break;
                case "all_performance":
                    new AllPerformanceTest().Run();
                    break;
                case "optimisation":
                    new Optimisation().Run();
                    break;
                default:
                    Console.Error.WriteLine("Argument is invalid.");
                    break;
            }
        }
    }
}