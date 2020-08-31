using Experimenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Experiments
{
    [TestClass]
    public class ConcurrentPerformance : IExperiment
    {
        [Parameters(new object[] {"Examples/nug20.dat", "Examples/nug30.dat", "Examples/sko42.dat"}, 3)]
        public string Problem;

        [Parameters(new object[] {5, 10, 15, 20}, 2)]
        public int AntCount;

        [Parameters(new object[] {5, 10, 15, 20}, 1)]
        public int StopThreshold;

        public double Experiment()
        {
            return Utils.RunExperiments($"-c {AntCount} -s {StopThreshold} {Problem}".Split(' '));
        }

        [TestMethod]
        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 10);
        }
    }
}