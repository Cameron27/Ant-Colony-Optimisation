using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ReplicatedPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat"}, 1)]
        public string Problem = "Examples/sko42.dat";

        public override double RunExperiment()
        {
            return Utils.RunExperiments(
                $"-a Replicated -t 4 {Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 300);
        }
    }
}