// CameronSalisbury_1293897

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Tests
{
    [TestClass]
    public class SolutionTests
    {
        [TestMethod]
        public void EvaluateFitnessTest()
        {
            Problem p = Problem.CreateFromFile("Examples/nug12.dat");
            (Solution s, int fitness) = Solution.CreateFromFile("Examples/nug12.sol", p);

            Assert.AreEqual(s.Fitness, fitness);
        }
    }
}