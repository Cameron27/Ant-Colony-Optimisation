// CameronSalisbury_1293897

using System.Collections.Generic;
using System.Linq;

namespace QuadraticAssignmentSolver.Experiments
{
    public static class Utils
    {
        public static readonly Dictionary<string, double> ProblemTimeDictionary =
            new Dictionary<string, double>(new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Examples/sko42.dat", 2),
                new KeyValuePair<string, double>("Examples/sko49.dat", 4),
                new KeyValuePair<string, double>("Examples/sko64.dat", 10),
                new KeyValuePair<string, double>("Examples/sko81.dat", 20),
                new KeyValuePair<string, double>("Examples/sko100a.dat", 50)
            });

        public static double[] FitnessesAndIterations(this (Solution[] Solutions, double Iterations) result)
        {
            (Solution[] solutions, double iterations) = result;
            return solutions.Select(s => (double) s.Fitness).Append(iterations).ToArray();
        }
    }
}