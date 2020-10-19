using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Tests
{
    [TestClass]
    public class AntColonyOptimiserTests
    {
        [TestMethod]
        public void SearchTest()
        {
            AntColonyOptimiser aco = new AntColonyOptimiser("Examples/nug12.dat");

            Solution result = aco.SequentialSearch(5, 1).Solution;

            result.DisplayResult();

            Solution[] results = aco.SequentialSearch(5, 1, 5).Solutions;
            Assert.AreEqual(5, results.Length);
            for (int i = 1; i < results.Length; i++) Assert.IsTrue(results[i - 1].Fitness >= results[i].Fitness);
        }

        [TestMethod]
        public void ReplicatedSearchTest()
        {
            AntColonyOptimiser aco = new AntColonyOptimiser("Examples/nug12.dat");

            Solution result = aco.ReplicatedSearch(5, 1, Environment.ProcessorCount).Solution;

            result.DisplayResult();

            Solution[] results = aco.ReplicatedSearch(5, 1, Environment.ProcessorCount, 5).Solutions;
            Assert.AreEqual(5, results.Length);
            for (int i = 1; i < results.Length; i++) Assert.IsTrue(results[i - 1].Fitness >= results[i].Fitness);
        }

        [TestMethod]
        public void SynchronousSearchTest()
        {
            AntColonyOptimiser aco = new AntColonyOptimiser("Examples/nug12.dat");

            Solution result = aco.SynchronousSearch(5, 1, Environment.ProcessorCount).Solution;

            result.DisplayResult();

            Solution[] results = aco.SynchronousSearch(5, 1, Environment.ProcessorCount, 5).Solutions;
            Assert.AreEqual(5, results.Length);
            for (int i = 1; i < results.Length; i++) Assert.IsTrue(results[i - 1].Fitness >= results[i].Fitness);
        }

        [TestMethod]
        public void LocalSearchTest()
        {
            Problem problem = Problem.CreateFromFile("Examples/sko42.dat");

            Solution s1 = new Solution(problem);
            int[] init1 =
            {
                16, 4, 7, 5, 26, 33, 40, 35, 0, 17, 41, 3, 13, 2, 8, 32, 38, 21, 31, 10, 39, 1, 20, 27, 28, 34, 23, 12,
                15, 22, 19, 30, 36, 14, 37, 25, 29, 24, 11, 6, 18, 9
            };
            int[] final1 =
            {
                22, 20, 4, 2, 24, 6, 40, 7, 36, 0, 37, 39, 28, 5, 21, 23, 25, 17, 31, 10, 16, 27, 35, 1, 13, 19, 33, 41,
                29, 38, 34, 18, 14, 11, 9, 32, 15, 26, 3, 8, 30, 12
            };

            Solution s2 = new Solution(problem);
            int[] init2 =
            {
                15, 16, 25, 3, 10, 31, 0, 9, 28, 5, 11, 41, 7, 27, 33, 37, 17, 40, 36, 22, 8, 4, 26, 39, 2, 14, 19, 32,
                24, 18, 34, 23, 20, 12, 21, 13, 1, 38, 6, 30, 35, 29
            };
            int[] final2 =
            {
                24, 5, 28, 33, 10, 39, 7, 40, 2, 17, 6, 37, 0, 20, 4, 41, 11, 16, 31, 14, 8, 15, 9, 18, 19, 25, 23, 3,
                29, 38, 34, 30, 13, 36, 32, 22, 35, 1, 26, 27, 21, 12
            };

            Solution s3 = new Solution(problem);
            int[] init3 =
            {
                35, 15, 22, 4, 12, 14, 2, 38, 37, 23, 8, 31, 28, 36, 5, 29, 26, 24, 34, 21, 39, 10, 3, 1, 16, 13, 11,
                30, 17, 19, 27, 0, 33, 32, 6, 9, 18, 41, 25, 40, 20, 7
            };
            int[] final3 =
            {
                22, 35, 21, 23, 2, 0, 5, 1, 27, 17, 13, 31, 36, 7, 15, 38, 29, 3, 32, 37, 20, 26, 19, 25, 34, 8, 14, 39,
                12, 30, 11, 10, 33, 28, 24, 9, 18, 4, 41, 6, 16, 40
            };

            for (int i = 0; i < s1.Size; i++)
            {
                s1.SetFacility(i, init1[i]);
                s2.SetFacility(i, init2[i]);
                s3.SetFacility(i, init3[i]);
            }

            s1 = AntColonyOptimiser.LocalSearch(s1);
            s2 = AntColonyOptimiser.LocalSearch(s2);
            s3 = AntColonyOptimiser.LocalSearch(s3);

            for (int i = 0; i < s1.Size; i++)
            {
                Assert.AreEqual(final1[i], s1.GetFacility(i));
                Assert.AreEqual(final2[i], s2.GetFacility(i));
                Assert.AreEqual(final3[i], s3.GetFacility(i));
            }
        }
    }
}