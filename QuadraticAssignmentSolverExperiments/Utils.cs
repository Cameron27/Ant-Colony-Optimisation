using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace QuadraticAssignmentSolver.Experiments
{
    public static class Utils
    {
        public static (Result[], Stats) RunExperiments(string[] args, int iterations)
        {
            Stopwatch sw = new Stopwatch();

            TextWriter stdout = Console.Out;
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            Console.SetOut(tw);

            Result[] results = new Result[iterations];


            for (int i = 0; i < iterations; i++)
            {
                sw.Start();
                Program.Main(args);
                sw.Stop();

                int fitness = int.Parse(sb.ToString().Split(Environment.NewLine)[1].Split(' ')[2]);
                long time = sw.ElapsedMilliseconds;

                results[i] = new Result(fitness, time);

                sw.Reset();
                sb.Clear();
            }

            Console.SetOut(stdout);

            return (results, new Stats(results));
        }
    }
}