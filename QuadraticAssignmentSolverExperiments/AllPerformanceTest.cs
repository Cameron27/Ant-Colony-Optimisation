using System;
using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class AllPerformanceTest : Experiment
    {
        public enum TestAlgorithm
        {
            Concurrent,
            Replicated2,
            Replicated4,
            Replicated6,
            Replicated12,
            Synchronous2,
            Synchronous3,
            Synchronous4,
            Synchronous5
        }

        private const int DivisionCount = 10;
        private const int Iterations = 10;

        [Parameters(new object[]
        {
            TestAlgorithm.Concurrent,
            TestAlgorithm.Replicated2,
            TestAlgorithm.Replicated4,
            TestAlgorithm.Replicated6,
            TestAlgorithm.Replicated12,
            TestAlgorithm.Synchronous2,
            TestAlgorithm.Synchronous3,
            TestAlgorithm.Synchronous4,
            TestAlgorithm.Synchronous5
        })]
        public TestAlgorithm Algorithm;

        [Parameters(new object[] {"Examples/sko42.dat"}, 1)]
        public string Problem;

        public override double[] RunExperiment()
        {
            double[] result = Algorithm switch
            {
                TestAlgorithm.Concurrent => new AntColonyOptimiser(Problem)
                    .ConcurrentSearch(5, Utils.ProblemTimeDictionary[Problem], DivisionCount)
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
                TestAlgorithm.Synchronous3 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 3, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous4 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 4, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous5 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 5, DivisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return result;
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, Iterations);
        }
    }
}