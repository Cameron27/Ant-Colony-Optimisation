using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ReplicatedParallelOptimisation : Experiment
    {
        [Parameters(new object[] {"Examples/sko42.dat", "Examples/sko49.dat"}, 1)]
        public string A_Problem = "Examples/sko42.dat";

        // [Parameters(new object[] {0.5d, 1d, 1.5d, 2d, 2.5d, 3d})]
        public double B_FitnessWeight = 2;

        // [Parameters(new object[] {0.5d, 1d, 1.5d, 2d, 2.5d, 3d}, -1)]
        public double C_PheromoneWeight = 1.5;

        [Parameters(new object[] {50d, 10d, 5d, 1d, 0.5d, 0.1d}, -2)]
        public double D_InitialValue = 5;

        [Parameters(new object[] {0.7d, 0.6d, 0.5d, 0.4d, 0.3d, 0.2d, 0.1d}, -3)]
        public double E_EvaporationRate = 0.4;

        public override double RunExperiment()
        {
            AntColonyOptimiser.FitnessWeight = B_FitnessWeight;
            AntColonyOptimiser.PheromoneWeight = C_PheromoneWeight;
            PheromoneTable.InitialValue = D_InitialValue;
            PheromoneTable.EvaporationRate = E_EvaporationRate;
            return Utils.RunExperiments(
                $"-a Replicated -t 4 {A_Problem}".Split(' '));
        }

        public void Run()
        {
            Experimenter.Experimenter.RunExperiment(this, 50, "synchronous_table");
        }
        
    }
}