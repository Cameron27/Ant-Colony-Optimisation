using System.Collections.Generic;

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
                new KeyValuePair<string, double>("Examples/sko100a.dat", 40),
                new KeyValuePair<string, double>("Examples/sko100b.dat", 40),
                new KeyValuePair<string, double>("Examples/sko100c.dat", 40),
                new KeyValuePair<string, double>("Examples/sko100d.dat", 40),
                new KeyValuePair<string, double>("Examples/sko100e.dat", 40),
                new KeyValuePair<string, double>("Examples/sko100f.dat", 40)
            });
    }
}