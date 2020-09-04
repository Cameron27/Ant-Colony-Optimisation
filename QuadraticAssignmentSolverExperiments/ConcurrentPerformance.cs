using Experimenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Experiments
{
    [TestClass]
    public class ConcurrentPerformance : IExperiment
    {
        [Parameters(new object[] {"Examples/nug30.dat"}, 3)]
        public string Problem;

        [Parameters(new object[] {10}, 2)] public int AntCount;

        [Parameters(new object[] {10}, 1)] public int StopThreshold;

        public double Experiment()
        {
            return Utils.RunExperiments($"-c {AntCount} -s {StopThreshold} {Problem}".Split(' '));
        }

        [TestMethod]
        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 20, false);
        }
    }
}