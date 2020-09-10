using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ConcurrentPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat"})]
        public string A_Problem = "Examples/sko42.dat";

        public override double RunExperiment()
        {
            return Utils.RunExperiments(
                $"{A_Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50, false);
        }
    }
}