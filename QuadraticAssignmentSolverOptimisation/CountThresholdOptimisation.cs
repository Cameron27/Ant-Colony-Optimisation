using Experimenter;
using QuadraticAssignmentSolver;
using QuadraticAssignmentSolver.Optimisation;

namespace QuadraticAssignmentSolverOptimisation
{
    public class CountThresholdOptimisation : Experiment
    {
        public string A_Problem = "Examples/sko42.dat";

        [Parameters(new object[] {5, 10, 15, 20, 25})]
        public int B_AntCount;

        [Parameters(new object[] {5, 10, 15, 20, 25, 30, 35, 40})]
        public int C_StopThreshold;

        public double D_FitnessWeight = 4;

        public double E_PheromoneWeight = 3;

        public double F_InitialValue = 0.05;

        public double G_EvaporationRate = 0.7;

        public override double RunExperiment()
        {
            AntColonyOptimiser.FitnessWeight = D_FitnessWeight;
            AntColonyOptimiser.PheromoneWeight = E_PheromoneWeight;
            PheromoneTable.InitialValue = F_InitialValue;
            PheromoneTable.EvaporationRate = G_EvaporationRate;
            return Utils.RunExperiments(
                $"-c {B_AntCount.ToString()} -s {C_StopThreshold.ToString()} {A_Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50);
        }
    }
}