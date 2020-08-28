using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Tests
{
    [TestClass]
    public class PheromoneTableTests
    {
        [TestMethod]
        public void DepositPheromonesTest()
        {
            Problem p = Problem.CreateFromFile("Examples/nug3.dat");
            Solution s1 = new Solution(p);
            s1.SetFacility(0, 0);
            s1.SetFacility(1, 1);
            s1.SetFacility(2, 2);

            Assert.AreEqual(s1.EvaluateFitness(), 24);

            Solution s2 = new Solution(p);
            s2.SetFacility(0, 0);
            s2.SetFacility(1, 2);
            s2.SetFacility(2, 1);

            Assert.AreEqual(s2.EvaluateFitness(), 30);

            PheromoneTable pt = new PheromoneTable(p);

            for (int location = 0; location < p.Size; location++)
                for (int facility = 0; facility < p.Size; facility++)
                    Assert.AreEqual(pt.GetPheromones(location, facility), PheromoneTable.InitialValue);

            pt.DepositPheromones(new[] { (s1, s1.EvaluateFitness()), (s2, s2.EvaluateFitness()) });

            for (int location = 0; location < p.Size; location++)
                for (int facility = 0; facility < p.Size; facility++)
                    Assert.AreEqual(pt.GetPheromones(location, facility),
                        PheromoneTable.EvaporationRate * PheromoneTable.InitialValue
                        + (s1.GetFacility(location) == facility ? 1d / s1.EvaluateFitness() : 0)
                        + (s2.GetFacility(location) == facility ? 1d / s2.EvaluateFitness() : 0));
        }
    }
}