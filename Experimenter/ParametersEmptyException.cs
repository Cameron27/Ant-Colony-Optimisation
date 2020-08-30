using System;

namespace Experimenter
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