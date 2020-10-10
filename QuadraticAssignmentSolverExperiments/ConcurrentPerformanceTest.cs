using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class ConcurrentPerformanceTest : Experiment
    {
        // [Parameters(new object[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})]
        public int AntCount = 5;

        // [Parameters(new object[] {0.4d, 0.5d, 0.6d, 0.7d, 0.8d, 0.9d})]
        public double EvaporationRate = 0.6;

        // [Parameters(new object[] {1d, 2d, 3d, 4d, 5d}, -1)]
        public double FitnessWeight = 3;

        // [Parameters(new object[] {2, 4, 6, 8, 10, 12, 14, 16, 18, 20})]
        public int GlobalBestDepositFreq = 16;

        // [Parameters(new object[] {1d, 2d, 3d, 4d, 5d})]
        public double PheromoneWeight = 1;

        // [Parameters(new object[] {0.01d, 0.02d, 0.03d, 0.04d, 0.05d, 0.06d, 0.07d, 0.08d, 0.09d, 0.1d})]
        public double ProbBest = 0.1;

        [Parameters(new object[] {"Examples/sko49.dat"}, 1)]
        public string Problem;

        public override double[] RunExperiment()
        {
            PheromoneTable.EvaporationRate = EvaporationRate;
            PheromoneTable.ProbBest = ProbBest;
            AntColonyOptimiser.PheromoneWeight = PheromoneWeight;
            AntColonyOptimiser.FitnessWeight = FitnessWeight;
            AntColonyOptimiser.GlobalBestDepositFreq = GlobalBestDepositFreq;
            return new AntColonyOptimiser(Problem)
                .ConcurrentSearch(5, Utils.ProblemTimeDictionary[Problem], 10)
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}