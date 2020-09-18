using System;
using System.Linq;
using Experimenter;
using QuadraticAssignmentSolverOptimisation;

namespace QuadraticAssignmentSolver.Optimisation
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
            Synchronous4,
            Synchronous6,
            Synchronous12,
            CourseGrained2,
            CourseGrained4,
            CourseGrained6,
            CourseGrained12
        }

        [Parameters(new object[] { 
            TestAlgorithm.Concurrent,
            TestAlgorithm.Replicated2,
            TestAlgorithm.Replicated4,
            TestAlgorithm.Replicated6,
            TestAlgorithm.Replicated12,
            TestAlgorithm.Synchronous2,
            TestAlgorithm.Synchronous4,
            TestAlgorithm.Synchronous6,
            TestAlgorithm.Synchronous12,
            // TestAlgorithm.CourseGrained2,
            // TestAlgorithm.CourseGrained4,
            // TestAlgorithm.CourseGrained6,
            // TestAlgorithm.CourseGrained12
            
        })] 
        public TestAlgorithm Algorithm;

        [Parameters(new object[] {"Examples/sko42.dat"}, 1)]
        public string Problem;

        private int _divisionCount = 10;
        private int _iterations = 10;

        public override double[] RunExperiment()
        {
            double[] result = Algorithm switch
            {
                TestAlgorithm.Concurrent => new AntColonyOptimiser(Problem)
                    .Search(5, Utils.ProblemTimeDictionary[Problem], 1, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated2 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 2, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated4 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 4, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated6 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 6, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Replicated12 => new AntColonyOptimiser(Problem)
                    .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 12, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous2 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 2, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous4 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 4, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous6 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 6, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.Synchronous12 => new AntColonyOptimiser(Problem)
                    .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 12, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.CourseGrained2 => new AntColonyOptimiser(Problem)
                    .CourseGrainedSearch(5, Utils.ProblemTimeDictionary[Problem], 2, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.CourseGrained4 => new AntColonyOptimiser(Problem)
                    .CourseGrainedSearch(5, Utils.ProblemTimeDictionary[Problem], 4, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.CourseGrained6 => new AntColonyOptimiser(Problem)
                    .CourseGrainedSearch(5, Utils.ProblemTimeDictionary[Problem], 6, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                TestAlgorithm.CourseGrained12 => new AntColonyOptimiser(Problem)
                    .CourseGrainedSearch(5, Utils.ProblemTimeDictionary[Problem], 12, _divisionCount)
                    .Select(s => (double) s.Fitness)
                    .ToArray(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return result;
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, _iterations);
        }
    }
}