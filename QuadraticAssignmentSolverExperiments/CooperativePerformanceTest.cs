// CameronSalisbury_1293897

using System.Linq;
using QuadraticAssignmentSolver.Experiments.Experimenter;

namespace QuadraticAssignmentSolver.Experiments
{
    public class CooperativePerformanceTest : Experiment
    {
        [Parameters(new object[] {"Examples/sko49.dat"}, 1)]
        public string Problem;

        [Parameters(new object[] {5, 10, 15, 20, 25, 30})]
        public int shareCount = 15;

        public override double[] RunExperiment()
        {
            return new AntColonyOptimiser(Problem, AntColonyOptimiser.Algorithm.Cooperative)
                .CooperativeSearch(5, Utils.ProblemTimeDictionary[Problem], 4, shareCount, 1).Solutions
                .Select(s => (double) s.Fitness).ToArray();
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 75);
        }
    }
}