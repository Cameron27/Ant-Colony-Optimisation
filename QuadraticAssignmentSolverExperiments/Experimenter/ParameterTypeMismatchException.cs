using System;

namespace QuadraticAssignmentSolver.Experiments.Experimenter
{
    public class ParameterTypeMismatchException : Exception
    {
        public ParameterTypeMismatchException()
        {
        }

        public ParameterTypeMismatchException(string message) : base(message)
        {
        }
    }
}