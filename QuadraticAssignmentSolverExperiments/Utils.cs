﻿using System.Collections.Generic;

namespace QuadraticAssignmentSolver.Experiments
{
    public static class Utils
    {
        public static readonly Dictionary<string, double> ProblemTimeDictionary =
            new Dictionary<string, double>(new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Examples/sko42.dat", 2),
                new KeyValuePair<string, double>("Examples/sko49.dat", 3)
            });
    }
}