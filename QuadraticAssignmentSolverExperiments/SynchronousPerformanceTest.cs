using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class SynchronousPerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko49.dat"})]
        public string Problem; 
        
        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem, AntColonyOptimiser.Algorithm.Synchronous)
                .SynchronousSearch(5, Utils.ProblemTimeDictionary[Problem], 4, 1)
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}