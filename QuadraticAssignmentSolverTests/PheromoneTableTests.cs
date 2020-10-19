// CameronSalisbury_1293897

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Tests
{
    [TestClass]
    public class PheromoneTableTests
    {
        [TestMethod]
        public void DepositPheromonesTest()
        {
            Problem p = Problem.CreateFromFile("Examples/nug12.dat");
            (Solution s1, _) = Solution.CreateFromFile("Examples/nug12.sol", p);

            Assert.AreEqual(s1.Fitness, 578);

            PheromoneTable pt = new PheromoneTable(p, 0.9, 0.05);

            pt.UpdateMaxAndMin(s1);

            pt.DepositPheromones(s1);

            for (int location = 0; location < p.Size; location++)
            for (int facility = 0; facility < p.Size; facility++)
                Assert.AreEqual(0.01730103806, pt.GetPheromones(location, facility), 0.00001);

            pt.DepositPheromones(s1);

            for (int location = 0; location < p.Size; location++)
            for (int facility = 0; facility < p.Size; facility++)
                Assert.AreEqual(s1.GetFacility(location) == facility ? 0.01730103806 : 0.01557093425,
                    pt.GetPheromones(location, facility), 0.00001);
        }

        [TestMethod]
        public void UpdateMaxAndMinTest()
        {
            Problem p = Problem.CreateFromFile("Examples/nug12.dat");
            (Solution s1, _) = Solution.CreateFromFile("Examples/nug12.sol", p);

            Assert.AreEqual(s1.Fitness, 578);

            PheromoneTable pt = new PheromoneTable(p, 0.9, 0.05);

            pt.UpdateMaxAndMin(s1);

            Assert.AreEqual(0.01730103806, pt.GetMaxAndMin().Max, 0.00001);
            Assert.AreEqual(0.000981207066, pt.GetMaxAndMin().Min, 0.0000001);
        }
    }
}