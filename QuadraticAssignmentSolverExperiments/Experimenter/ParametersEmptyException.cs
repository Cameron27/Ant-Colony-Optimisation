// CameronSalisbury_1293897

using System;

namespace QuadraticAssignmentSolver.Experiments.Experimenter
{
    public class ParametersEmptyException : Exception
    {
        public ParametersEmptyException()
        {
        }

        public ParametersEmptyException(string message) : base(message)
        {
        }
    }
}