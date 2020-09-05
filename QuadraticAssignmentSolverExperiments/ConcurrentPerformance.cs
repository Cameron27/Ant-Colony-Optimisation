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

        [Parameters(new object[] {1d, 2d, 3d, 4d})]
        public double FitnessWeight;

        [Parameters(new object[] {1d, 2d, 3d, 4d})]
        public double PheromoneWeight;

        [Parameters(new object[] {0.01d, 0.001d, 0.0001d}, -1)]
        public double InitialValue;

        [Parameters(new object[] {0.9d, 0.8d, 0.7d, 0.6d}, -1)]
        public double EvaporationRate;

        public double Experiment()
        {
            AntColonyOptimiser.FitnessWeight = FitnessWeight;
            AntColonyOptimiser.PheromoneWeight = PheromoneWeight;
            PheromoneTable.InitialValue = InitialValue;
            PheromoneTable.EvaporationRate = EvaporationRate;
            return Utils.RunExperiments($"-c {AntCount} -s {StopThreshold} {Problem}".Split(' '));
        }

        [TestMethod]
        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 10);
        }
    }
}