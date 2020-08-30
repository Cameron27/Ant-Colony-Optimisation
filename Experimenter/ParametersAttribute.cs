using System;

namespace Experimenter
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ParametersAttribute : Attribute
    {
        public readonly object[] Parameters;

        public ParametersAttribute()
        {
            Parameters = new object[0];
        }

        public ParametersAttribute(params object[] parameters)
        {
            Parameters = parameters;
        }
    }
}