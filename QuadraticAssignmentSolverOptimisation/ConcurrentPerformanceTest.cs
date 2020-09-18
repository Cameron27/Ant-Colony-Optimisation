using System.Linq;
using Experimenter;
using QuadraticAssignmentSolverOptimisation;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ConcurrentPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat"})]
        public string Problem;

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