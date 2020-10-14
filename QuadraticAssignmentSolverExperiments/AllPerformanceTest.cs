using System;
using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class AllPerformanceTest : Experiment
    {
        public enum TestAlgorithm
        {
            Sequential,
            Replicated2,
            Replicated4,
            Replicated6,
            Replicated12,
            Synchronous2,
            Synchronous4,
            Synchronous6,
            Synchronous12,
            Cooperative2,
            Cooperative4,
            Cooperative6,
            Cooperative12
        }

        private const int DivisionCount = 10;
        private const int Iterations = 50;

        [Parameters(new object[]
        {
            TestAlgorithm.Sequential,
            TestAlgorithm.Replicated2,
            TestAlgorithm.Replicated4,
            TestAlgorithm.Replicated6,
            TestAlgorithm.Replicated12,
            TestAlgorithm.Synchronous2,
            TestAlgorithm.Synchronous4,
            TestAlgorithm.Synchronous6,
            TestAlgorithm.Synchronous12,
            TestAlgorithm.Cooperative2,
            TestAlgorithm.Cooperative4,
            TestAlgorithm.Cooperative6,
            TestAlgorithm.Cooperative12
        })]
        public TestAlgorithm Algorithm;

        [Parameters(
            new object[]
            {
                "Examples/sko49.dat", "Examples/sko64.dat", "Examples/sko81.dat", "Examples/sko100a.dat",
                "Examples/sko100b.dat"
            }, 1)]
        public string Problem;

        public override double[] RunExperiment()
        {
            double[] result = Algorithm switch
            {
                TestAlgorithm.Sequential => new AntColonyOptimiser(Problem)
                    .SequentialSearch(5, Utils.ProblemTimeDictionary[Problem], DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated2 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 2, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated4 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 4, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated6 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 6, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated12 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 12, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous2 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 2, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous4 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 4, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous6 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 6, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous12 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 12, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Cooperative2 => new AntColonyOptimiser(Problem)
                    .CooperativeSearch(5, Utils.ProblemTimeDictionary[Problem], 2, 10, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Cooperative4 => new AntColonyOptimiser(Problem)
                    .CooperativeSearch(5, Utils.ProblemTimeDictionary[Problem], 4, 10, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Cooperative6 => new AntColonyOptimiser(Problem)
                    .CooperativeSearch(5, Utils.ProblemTimeDictionary[Problem], 6, 10, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Cooperative12 => new AntColonyOptimiser(Problem)
                    .CooperativeSearch(5, Utils.ProblemTimeDictionary[Problem], 12, 10, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return result;
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, Iterations, "all.results");
        }
    }
}