using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Experiments
{
    [TestClass]
    public class ConcurrentPerformance
    {
        [TestMethod]
        public void Experiment()
        {
            (Result[] results, Stats stats) = Utils.RunExperiments(new[] {"Examples/nug20.dat"}, 20);

            Console.WriteLine(stats);
        }
    }
}