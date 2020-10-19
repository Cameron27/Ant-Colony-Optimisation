// CameronSalisbury_1293897

using System;

namespace QuadraticAssignmentSolver.Experiments.Experimenter
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ParametersAttribute : Attribute
    {
        /// <summary>
        ///     An array of parameters associated with the field.
        /// </summary>
        public readonly object[] Parameters;

        /// <summary>
        ///     The priority of the field.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        ///     Initialise a new instance of the <code>ParametersAttribute</code> class with no parameters and priority 0.
        /// </summary>
        public ParametersAttribute()
        {
            Parameters = new object[0];
        }

        /// <summary>
        ///     Initialise a new instance of the <code>ParametersAttribute</code> class with the given parameters and priority.
        /// </summary>
        /// <param name="parameters">The array of parameters for the field.</param>
        /// <param name="priority">The priority of the field.</param>
        public ParametersAttribute(object[] parameters, int priority = 0)
        {
            Parameters = parameters;
            Priority = priority;
        }
    }
}