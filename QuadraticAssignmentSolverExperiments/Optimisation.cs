using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class Optimisation : Experiment
    {
        [Parameters(new object[]
        {
            AllPerformanceTest.TestAlgorithm.Sequential, AllPerformanceTest.TestAlgorithm.Replicated4,
            AllPerformanceTest.TestAlgorithm.Synchronous4, AllPerformanceTest.TestAlgorithm.Cooperative4
        }, 1)]
        public AllPerformanceTest.TestAlgorithm Algorithm;

        // [Parameters(new object[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10})]
        public int AntCount = 5;

        // [Parameters(new object[] {0.1d, 0.2d, 0.3d, 0.4d, 0.5d, 0.6d, 0.7d, 0.8d, 0.9d})]
        public double EvaporationRate = 0.6;

        // [Parameters(new object[] {1d, 2d, 3d, 4d, 5d}, -1)]
        public double FitnessWeight = 3;

        // [Parameters(new object[] {8, 10, 12, 14, 16, 18, 20})]
        public int GlobalBestDepositFreq = 16;

        // [Parameters(new object[] {1d, 2d, 3d})]
        public double PheromoneWeight = 1;

        // [Parameters(new object[] {0.02d, 0.04d, 0.06d, 0.08d, 0.1d, 0.12d, 0.14d, 0.16d})]
        public double ProbBest = 0.1;

        [Parameters(new object[] {"Examples/sko49.dat"}, 2)]
        public string Problem;

        public override double[] RunExperiment()
        {
            AntColonyOptimiser aco = new AntColonyOptimiser(Problem)
            {
                EvaporationRate = EvaporationRate,
                ProbBest = ProbBest,
                PheromoneWeight = PheromoneWeight,
                FitnessWeight = FitnessWeight,
                GlobalBestDepositFreq = GlobalBestDepositFreq
            };

            switch (Algorithm)
            {
                case AllPerformanceTest.TestAlgorithm.Sequential:
                    aco.PheromoneWeight = 1;
                    aco.FitnessWeight = 3;
                    aco.GlobalBestDepositFreq = 16;
                    aco.EvaporationRate = 0.5;
                    aco.ProbBest = 0.1;
                    AntCount = 5;
                    break;
                case AllPerformanceTest.TestAlgorithm.Replicated4:
                    aco.PheromoneWeight = 1;
                    aco.FitnessWeight = 3;
                    aco.GlobalBestDepositFreq = 12;
                    aco.EvaporationRate = 0.6;
                    aco.ProbBest = 0.06;
                    AntCount = 5;
                    break;
                case AllPerformanceTest.TestAlgorithm.Synchronous4:
                    aco.PheromoneWeight = 1;
                    aco.FitnessWeight = 3;
                    aco.GlobalBestDepositFreq = 14;
                    aco.EvaporationRate = 0.7;
                    aco.ProbBest = 0.08;
                    AntCount = 5;
                    break;
                case AllPerformanceTest.TestAlgorithm.Cooperative4:
                    aco.PheromoneWeight = 1;
                    aco.FitnessWeight = 3;
                    aco.GlobalBestDepositFreq = 14;
                    aco.EvaporationRate = 0.3;
                    aco.ProbBest = 0.08;
                    AntCount = 5;
                    break;
            }

#pragma warning disable 8509
            return Algorithm switch
#pragma warning restore 8509
            {
                AllPerformanceTest.TestAlgorithm.Sequential =>
                    aco.SequentialSearch(5, Utils.ProblemTimeDictionary[Problem], 1).Select(s => (double) s.Fitness)
                        .ToArray(),
                AllPerformanceTest.TestAlgorithm.Replicated4 =>
                    aco.ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 4, 1).Select(s => (double) s.Fitness)
                        .ToArray(),
                AllPerformanceTest.TestAlgorithm.Synchronous4 =>
                    aco.SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 4, 1).Select(s => (double) s.Fitness)
                        .ToArray(),
                AllPerformanceTest.TestAlgorithm.Cooperative4 =>
                    aco.CooperativeSearch(5, Utils.ProblemTimeDictionary[Problem], 10, 4, 1)
                        .Select(s => (double) s.Fitness).ToArray()
            };
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50, meanOnly: true);
        }
    }
}