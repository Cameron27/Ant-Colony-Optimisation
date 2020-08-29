using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Tests
{
    [TestClass]
    public class ProblemTests
    {
        [TestMethod]
        public void CreateFromFileTest()
        {
            int[,] expectedDistances =
            {
                {0, 1, 2, 3, 1, 2, 3, 4, 2, 3, 4, 5},
                {1, 0, 1, 2, 2, 1, 2, 3, 3, 2, 3, 4},
                {2, 1, 0, 1, 3, 2, 1, 2, 4, 3, 2, 3},
                {3, 2, 1, 0, 4, 3, 2, 1, 5, 4, 3, 2},
                {1, 2, 3, 4, 0, 1, 2, 3, 1, 2, 3, 4},
                {2, 1, 2, 3, 1, 0, 1, 2, 2, 1, 2, 3},
                {3, 2, 1, 2, 2, 1, 0, 1, 3, 2, 1, 2},
                {4, 3, 2, 1, 3, 2, 1, 0, 4, 3, 2, 1},
                {2, 3, 4, 5, 1, 2, 3, 4, 0, 1, 2, 3},
                {3, 2, 3, 4, 2, 1, 2, 3, 1, 0, 1, 2},
                {4, 3, 2, 3, 3, 2, 1, 2, 2, 1, 0, 1},
                {5, 4, 3, 2, 4, 3, 2, 1, 3, 2, 1, 0}
            };
            int[,] expectedFlows =
            {
                {0, 5, 2, 4, 1, 0, 0, 6, 2, 1, 1, 1},
                {5, 0, 3, 0, 2, 2, 2, 0, 4, 5, 0, 0},
                {2, 3, 0, 0, 0, 0, 0, 5, 5, 2, 2, 2},
                {4, 0, 0, 0, 5, 2, 2, 10, 0, 0, 5, 5},
                {1, 2, 0, 5, 0, 10, 0, 0, 0, 5, 1, 1},
                {0, 2, 0, 2, 10, 0, 5, 1, 1, 5, 4, 0},
                {0, 2, 0, 2, 0, 5, 0, 10, 5, 2, 3, 3},
                {6, 0, 5, 10, 0, 1, 10, 0, 0, 0, 5, 0},
                {2, 4, 5, 0, 0, 1, 5, 0, 0, 0, 10, 10},
                {1, 5, 2, 0, 5, 5, 2, 0, 0, 0, 5, 0},
                {1, 0, 2, 5, 1, 4, 3, 5, 10, 5, 0, 2},
                {1, 0, 2, 5, 1, 0, 3, 0, 10, 0, 2, 0}
            };

            Problem p = Problem.CreateFromFile("Examples/nug12.dat");

            for (int y = 0; y < p.Size; y++)
            for (int x = 0; x < p.Size; x++)
            {
                Assert.AreEqual(p.GetDistance(x, y), expectedDistances[y, x]);
                Assert.AreEqual(p.GetFlow(x, y), expectedFlows[y, x]);
            }
        }

        [TestMethod]
        public void CreateFromFileFormatExceptionTest()
        {
            Assert.AreEqual(
                Assert.ThrowsException<FormatException>(() => Problem.CreateFromFile("Examples/nug12-fail1.dat"))
                    .Message,
                "A value in the file is not a number.");
            Assert.AreEqual(
                Assert.ThrowsException<FormatException>(() => Problem.CreateFromFile("Examples/nug12-fail2.dat"))
                    .Message,
                "File contains 288 values, 289 values were expected for a problem of size 12.");
            Assert.AreEqual(
                Assert.ThrowsException<FormatException>(() => Problem.CreateFromFile("Examples/nug12-fail3.dat"))
                    .Message,
                "File contains 290 values, 289 values were expected for a problem of size 12.");
            Assert.AreEqual(
                Assert.ThrowsException<FormatException>(() => Problem.CreateFromFile("Examples/nug12-fail4.dat"))
                    .Message,
                "File contains no values.");
        }
    }
}