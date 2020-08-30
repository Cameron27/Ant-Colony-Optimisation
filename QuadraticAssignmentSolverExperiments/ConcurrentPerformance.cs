using Experimenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Experiments
{
    [TestClass]
    public class ConcurrentPerformance : IExperiment
    {
        [Parameters(5, 10, 15, 20)] public int AntCount;

        [Parameters("Examples/nug12.dat")] public string Problem;

        [Parameters(5, 10, 15, 20)] public int StopThreshold;

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