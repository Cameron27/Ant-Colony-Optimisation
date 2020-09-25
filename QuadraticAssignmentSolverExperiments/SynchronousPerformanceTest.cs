using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class SynchronousPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat", "Examples/sko49.dat"})]
        public string Problem;

        [Parameters(new object[] {1, 2, 3, 4, 5})]
        public int Threads;

        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem)
                .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], Threads, 10)
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}