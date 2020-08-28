using System;
using QuadraticAssignmentSolver.Utils;

namespace QuadraticAssignmentSolver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AntColonyOptimiser aco;
            try
            {
                aco = new AntColonyOptimiser(args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            (Solution, int) result = aco.Search(5, 10);
            result.DisplayResult();
        }
    }
}