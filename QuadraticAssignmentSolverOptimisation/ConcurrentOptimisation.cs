using System;
using Experimenter;

namespace QuadraticAssignmentSolver.Optimisation
{
    public class ConcurrentOptimisation : IExperiment
    {
        public string A_Problem = "Examples/nug30.dat";
        public int B_AntCount = 20;
        public int C_StopThreshold = 20;
        public double D_FitnessWeight = 3;
        public double E_PheromoneWeight = 2;
        public double F_InitialValue = 0.5;
        public double G_EvaporationRate = 0.5;

        public double Experiment()
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
            Experimenter.Experimenter.RunOptimisation(this, 50, 3);
            Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", $"Ant Count: {B_AntCount.ToString()}",
                $"Stop Threshold: {C_StopThreshold.ToString()}", $"FitnessWeight: {D_FitnessWeight.ToString()}",
                $"Pheromone Weight: {E_PheromoneWeight.ToString()}", $"Initial Value: {F_InitialValue.ToString()}",
                $"Evaporation Weight: {G_EvaporationRate.ToString()}");
        }
    }
}