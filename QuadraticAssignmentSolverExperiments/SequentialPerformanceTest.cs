using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class SequentialPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko49.dat"}, 1)]
        public string Problem;

        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem)
                .SequentialSearch(5, Utils.ProblemTimeDictionary[Problem], 1).Solutions
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}