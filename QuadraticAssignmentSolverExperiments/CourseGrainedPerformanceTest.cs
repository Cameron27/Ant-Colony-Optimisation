using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class CourseGrainedPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat"})]
        public string Problem;

        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem)
                .CourseGrainedSearch(5, Utils.ProblemTimeDictionary[Problem], 4, 10)
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}