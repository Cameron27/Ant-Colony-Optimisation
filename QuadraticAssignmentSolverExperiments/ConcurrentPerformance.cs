using System;
using Experimenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuadraticAssignmentSolver.Experiments
{
    [TestClass]
    public class ConcurrentPerformance : IExperiment
    {
        public string Problem = "Examples/nug30.dat";

        [Parameters(new object[] {5, 10, 15, 20})]
        public int AntCount;

        [Parameters(new object[] {5, 10, 15, 20})]
        public int StopThreshold;

        [Parameters(new object[] {1d, 2d, 3d, 4d})]
        public double FitnessWeight;

        [Parameters(new object[] {1d, 2d, 3d, 4d})]
        public double PheromoneWeight;

        [Parameters(new object[] {0.01d, 0.001d, 0.0001d, 0d})]
        public double InitialValue;

        [Parameters(new object[] {0.9d, 0.8d, 0.7d, 0.6d})]
        public double EvaporationRate;

        public double Experiment()
        {
            AntColonyOptimiser.FitnessWeight = FitnessWeight;
            AntColonyOptimiser.PheromoneWeight = PheromoneWeight;
            PheromoneTable.InitialValue = InitialValue;
            PheromoneTable.EvaporationRate = EvaporationRate;
            return Utils.RunExperiments($"-c {AntCount} -s {StopThreshold} {Problem}".Split(' '));
        }

        [TestMethod]
        public void Run()
        {
            Experimenter.Experimenter.RunOptimisation(this, 50, 3);
            Console.WriteLine($"Ant Count: {AntCount}\n" +
                              $"Stop Threshold: {StopThreshold}\n" +
                              $"FitnessWeight: {FitnessWeight}\n" +
                              $"Pheromone Weight: {PheromoneWeight}\n" +
                              $"Initial Value: {InitialValue}\n" +
                              $"Evaporation Weight: {EvaporationRate}\n");
        }
    }
}