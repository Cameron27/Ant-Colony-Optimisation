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
        /// <summary>
        /// Run an experiment with all the possible combinations of specified parameters.
        /// </summary>
        /// <param name="instance">An object to use to perform experiments.</param>
        /// <param name="iterations">The number of iterations to perform per experiment.</param>
        /// <typeparam name="T">The type of the experiment.</typeparam>
        /// <exception cref="ParameterTypeMismatchException">A parameter does not match the type of its field.</exception>
        public static void RunExperiment<T>(T instance, int iterations) where T : IExperiment
        {
            // Get fields with a ParametersAttribute and the parameters for each of them
            Type type = instance.GetType();
            (FieldInfo field, object[] Parameters)[] fieldParameters = type.GetFields()
                .Where(field =>
                {
                    ParametersAttribute parametersAttribute = field.GetCustomAttribute<ParametersAttribute>();
                    if (parametersAttribute == null || parametersAttribute.Parameters.Length == 0) return false;

                    if (parametersAttribute.Parameters.Any(p => p.GetType() != field.FieldType))
                        throw new ParameterTypeMismatchException(
                            $"The field {field.Name} as a parameter of the wrong type.");

                    return true;
                }).OrderBy(field =>
                {
                    ParametersAttribute parametersAttribute = field.GetCustomAttribute<ParametersAttribute>();
                    return new Tuple<int, string>(parametersAttribute.Priority, field.Name);
                })
                .Select(field => (field, field.GetCustomAttribute<ParametersAttribute>().Parameters))
                .ToArray();

            // Setup table headers
            ColumnHeader[] headers = fieldParameters
                .Select(fp => fp.Item1.Name)
                .Reverse()
                .Union(new[]
                {
                    "Score Average", "Score Min", "Score Max", "Score SD", "Time Average", "Time Min", "Time Max",
                    "Time SD"
                })
                .Select(s => new ColumnHeader(s))
                .ToArray();

            // Create table
            Table table = new Table(headers) {Config = TableConfiguration.Unicode()};

            // Count up indices while all combinations have not been done
            int[] parameterIndices = new int[fieldParameters.Length];
            int[] fieldParameterCounts = fieldParameters.Select(fp => fp.Item2.Length).ToArray();
            while (parameterIndices[^1] != fieldParameterCounts[^1])
            {
                object[] row = new object[table.Columns.Count];

                // Set parameter values and fill in parameter values in table
                for (int i = 0; i < fieldParameters.Length; i++)
                {
                    (FieldInfo field, object[] parameters) = fieldParameters[i];
                    field.SetValue(instance, parameters[parameterIndices[i]]);

                    row[fieldParameters.Length - i - 1] = parameters[parameterIndices[i]];
                }

                // Perform iterations and save results
                double[] results = new double[iterations];
                long[] times = new long[iterations];
                for (int i = 0; i < iterations; i++)
                {
                    long startTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
                    results[i] = instance.Experiment();
                    times[i] = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond - startTime;
                }

                // Calculate and set stats in table
                object v1, v2, v3, v4;
                (v1, v2, v3, v4) = CalculateStats(results);
                row[^8] = v1;
                row[^7] = v2;
                row[^6] = v3;
                row[^5] = v4;
                (v1, v2, v3, v4) = CalculateStats(times);
                row[^4] = v1;
                row[^3] = v2;
                row[^2] = v3;
                row[^1] = v4;

                table.AddRow(row);

                // Count up parameter indices
                parameterIndices[0]++;
                for (int i = 0; i < parameterIndices.Length - 1; i++)
                {
                    if (parameterIndices[i] != fieldParameterCounts[i]) break;

                    parameterIndices[i] = 0;
                    parameterIndices[i + 1]++;
                }
            }

            // Print out table
            Console.WriteLine(table.ToString());
        }

        /// <summary>
        /// Calculate the average, min, max and standard deviation of an array of values.
        /// </summary>
        /// <param name="values">The array of values to calculate the stats with.</param>
        /// <returns>A tuple containing the average, min, max and standard deviation of the values in <code>values</code>.</returns>
        private static (double, double, double, double) CalculateStats(IReadOnlyCollection<double> values)
        {
            double min = values.Min(l => l);
            double max = values.Max(l => l);
            double average = values.Average();
            double sd = Math.Sqrt(1d / values.Count * values.Sum(l => Math.Pow(l - average, 2)));

            return (average, min, max, sd);
        }
        
        /// <summary>
        /// Calculate the average, min, max and standard deviation of an array of values.
        /// </summary>
        /// <param name="values">The array of values to calculate the stats with.</param>
        /// <returns>A tuple containing the average, min, max and standard deviation of the values in <code>values</code>.</returns>
        private static (double, long,long, double) CalculateStats(IReadOnlyCollection<long> values)
        {
            long min = values.Min(l => l);
            long max = values.Max(l => l);
            double average = values.Average();
            double sd = Math.Sqrt(1d / values.Count * values.Sum(l => Math.Pow(l - average, 2)));

            return (average, min, max, sd);
        }
    }
}