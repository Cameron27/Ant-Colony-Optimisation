using System.Linq;
using Experimenter;
using QuadraticAssignmentSolverOptimisation;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ReplicatedPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat"})]
        public string Problem ;

        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem)
                .ReplicatedSearch(5, Utils.ProblemTimeDictionary[Problem], 1, 10)
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}