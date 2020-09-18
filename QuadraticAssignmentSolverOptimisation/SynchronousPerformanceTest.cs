using System.Linq;
using Experimenter;
using QuadraticAssignmentSolverOptimisation;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class SynchronousPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat","Examples/sko49.dat"})]
        public string Problem ;

        [Parameters(new object[] {1, 2, 3, 4, 5})]
        public int Threads;

        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem)
                .Search(5, Utils.ProblemTimeDictionary[Problem], 1, 10)
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}