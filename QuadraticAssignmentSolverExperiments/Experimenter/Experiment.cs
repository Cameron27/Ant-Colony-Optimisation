using System.Collections.Generic;

namespace QuadraticAssignmentSolver.Experiments.Experimenter
{
    public abstract class Experiment
    {
        protected Experiment()
        {
            ScoresDictionary = new Dictionary<List<object>, double>();
        }

        internal Dictionary<List<object>, double> ScoresDictionary { get; }

        public abstract double[] RunExperiment();
    }
}