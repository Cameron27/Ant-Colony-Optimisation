using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ConcurrentOptimisation : Experiment
    {
        public string A_Problem = "Examples/sko42.dat";
        public int B_AntCount = 20;
        public int C_StopThreshold = 20;

        [Parameters(new object[] {1d, 2d, 3d, 4d, 5d})]
        public double D_FitnessWeight;

        [Parameters(new object[] {1d, 2d, 3d, 4d, 5d})]
        public double E_PheromoneWeight;

        [Parameters(new object[] {5d, 1d, 0.5d, 0.1d, 0.05d, 0.01d})]
        public double F_InitialValue;

        [Parameters(new object[] {0.9d, 0.8d, 0.7d, 0.6d, 0.5d, 0.4d, 0.3d})]
        public double G_EvaporationRate;

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
            Experimenter.Experimenter.RunOptimisation(this, 30, 5);
        }
        
    }
}