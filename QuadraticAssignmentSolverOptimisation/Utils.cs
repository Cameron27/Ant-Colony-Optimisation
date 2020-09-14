using System;
using System.IO;
using System.Text;

namespace QuadraticAssignmentSolver.Optimisation
{
    public static class Utils
    {
        public static int RunExperiments(string[] args)
        {
            TextWriter stdout = Console.Out;
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            Console.SetOut(tw);

            Program.OverrideParameters = false;
            Program.Main(args);
            int fitness = int.Parse(sb.ToString().Split(Environment.NewLine)[1].Split(' ')[2]);

            sb.Clear();

            Console.SetOut(stdout);

            return fitness;
        }
    }
}