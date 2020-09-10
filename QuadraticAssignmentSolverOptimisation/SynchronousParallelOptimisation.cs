using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class SynchronousParallelOptimisation : Experiment
    {
        public string A_Problem = "Examples/sko42.dat";

        [Parameters(new object[] {1d, 2d, 3d, 4d, 5d})]
        public double B_FitnessWeight;

        [Parameters(new object[] {1d, 2d, 3d, 4d, 5d})]
        public double C_PheromoneWeight;

        [Parameters(new object[] {5d, 1d, 0.5d, 0.1d, 0.05d, 0.01d})]
        public double D_InitialValue;

        [Parameters(new object[] {0.9d, 0.8d, 0.7d, 0.6d, 0.5d, 0.4d, 0.3d})]
        public double E_EvaporationRate;

        public override double RunExperiment()
        {
            AntColonyOptimiser.FitnessWeight = B_FitnessWeight;
            AntColonyOptimiser.PheromoneWeight = C_PheromoneWeight;
            PheromoneTable.InitialValue = D_InitialValue;
            PheromoneTable.EvaporationRate = E_EvaporationRate;
            return Utils.RunExperiments(
                $"-a Synchronous -t 4 {A_Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunOptimisation(this, 50, 5);
        }
        
    }
}