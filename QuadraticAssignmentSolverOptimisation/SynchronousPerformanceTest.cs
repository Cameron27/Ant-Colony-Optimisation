using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class SynchronousPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat","Examples/sko49.dat"}, 1)]
        public string Problem = "Examples/sko42.dat";

        [Parameters(new object[] {1, 2, 3, 4, 5})]
        public int Threads;

        public override double RunExperiment()
        {
            return Utils.RunExperiments(
                $"-a Synchronous -t {Threads} {Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}