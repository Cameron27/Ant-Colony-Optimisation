using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuadraticAssignmentSolver.Utils;

namespace QuadraticAssignmentSolver.Tests
{
    [TestClass]
    public class AntColonyOptimiserTests
    {
        [TestMethod]
        public void SearchTest()
        {
            AntColonyOptimiser aco = new AntColonyOptimiser("Examples/nug12.dat");

            (Solution, int) result = aco.Search(5, 20);

            result.DisplayResult();
        }

        [TestMethod]
        public void SynchronousParallelSearchTest()
        {
            AntColonyOptimiser aco = new AntColonyOptimiser("Examples/nug12.dat");

            (Solution, int) result = aco.SynchronousParallelSearch(5, 20, Environment.ProcessorCount);

            result.DisplayResult();
        }
    }
}