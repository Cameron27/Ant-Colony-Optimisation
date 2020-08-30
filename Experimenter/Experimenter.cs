using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BetterConsoleTables;

namespace Experimenter
{
    public static class Experimenter
    {
        public static void RunExperiment<T>(T instance, int iterations) where T : IExperiment
        {
            List<(FieldInfo, object[])> fieldParameters = new List<(FieldInfo, object[])>();

            Type type = instance.GetType();
            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                ParametersAttribute parametersAttribute = field.GetCustomAttribute<ParametersAttribute>();
                if (parametersAttribute == null || parametersAttribute.Parameters.Length == 0) continue;

                if (parametersAttribute.Parameters.Any(p => p.GetType() != field.FieldType))
                    throw new ParameterTypeMismatchException(
                        $"The field {field.Name} as a parameter of the wrong type.");

                fieldParameters.Add((field, parametersAttribute.Parameters));
            }

            ColumnHeader[] headers = fieldParameters
                .Select(fp => fp.Item1.Name)
                .Union(new[]
                {
                    "Score Average", "Score Min", "Score Max", "Score SD", "Time Average", "Time Min", "Time Max",
                    "Time SD"
                })
                .Select(s => new ColumnHeader(s))
                .ToArray();

            Table table = new Table(headers) {Config = TableConfiguration.Unicode()};

            int[] parameterIndices = new int[fieldParameters.Count];
            int[] fieldParameterCounts = fieldParameters.Select(fp => fp.Item2.Length).ToArray();
            while (parameterIndices[^1] != fieldParameterCounts[^1])
            {
                object[] row = new object[table.Columns.Count];

                for (int i = 0; i < fieldParameters.Count; i++)
                {
                    (FieldInfo field, object[] parameters) = fieldParameters[i];
                    field.SetValue(instance, parameters[parameterIndices[i]]);

                    row[i] = parameters[parameterIndices[i]];
                }

                double[] results = new double[iterations];
                long[] times = new long[iterations];
                for (int i = 0; i < iterations; i++)
                {
                    long startTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
                    results[i] = instance.Experiment();
                    times[i] = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond - startTime;
                }

                object v1, v2, v3, v4;
                (v1, v2, v3, v4) = CalculateStats(results, d => d, d => d);
                row[^8] = v1;
                row[^7] = v2;
                row[^6] = v3;
                row[^5] = v4;
                (v1, v2, v3, v4) = CalculateStats(times, l => (double) l, d => (long) d);
                row[^4] = v1;
                row[^3] = v2;
                row[^2] = v3;
                row[^1] = v4;

                table.AddRow(row);

                parameterIndices[0]++;
                for (int i = 0; i < parameterIndices.Length - 1; i++)
                {
                    if (parameterIndices[i] != fieldParameterCounts[i]) break;

                    parameterIndices[i] = 0;
                    parameterIndices[i + 1]++;
                }
            }

            Console.WriteLine(table.ToString());
        }

        private static (double, T, T, double) CalculateStats<T>(IEnumerable<T> results, Func<T, double> f,
            Func<double, T> g)
        {
            double[] values = results.Select(f).ToArray();

            T min = g(values.Min(l => l));
            T max = g(values.Max(l => l));
            double average = values.Average();
            double sd = Math.Sqrt(1d / values.Length * values.Sum(l => Math.Pow(l - average, 2)));

            return (average, min, max, sd);
        }
    }
}