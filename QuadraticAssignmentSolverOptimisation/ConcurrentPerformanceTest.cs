using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ConcurrentPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat", "Examples/sko49.dat"})]
        public string Problem = "Examples/sko42.dat";

        public override double RunExperiment()
        {
            return Utils.RunExperiments(
                $"{Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}