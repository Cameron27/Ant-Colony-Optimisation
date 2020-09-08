﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BetterConsoleTables;

namespace Experimenter
{
    public static class Experimenter
    {
        /// <summary>
        ///     Run an experiment with all the possible combinations of specified parameters.
        /// </summary>
        /// <param name="instance">An object to use to perform experiments.</param>
        /// <param name="iterations">The number of iterations to perform per experiment.</param>
        /// <typeparam name="T">The type of the experiment.</typeparam>
        /// <exception cref="ParameterTypeMismatchException">
        ///     A parameter does not match the type of its field.
        /// </exception>
        public static void RunExperiment<T>(T instance, int iterations, bool useFile = true) where T : Experiment
        {
            // Get fields with a ParametersAttribute and the parameters for each of them
            (FieldInfo Field, object[] Parameters)[] fieldParameters = GetFieldParameters(instance.GetType());

            // Load results from file
            string filename = GetFileName(fieldParameters);
            List<object[]> results = useFile ? LoadResults(filename) : new List<object[]>();

            int[] parameterIndices = new int[fieldParameters.Length];
            int[] fieldParameterCounts = fieldParameters.Select(fp => fp.Parameters.Length).ToArray();
            // Count up parameter indices equal to the number of results that were loaded
            for (int i = 0; i < results.Count; i++)
            {
                parameterIndices[0]++;
                for (int j = 0; j < parameterIndices.Length - 1; j++)
                {
                    if (parameterIndices[j] != fieldParameterCounts[j]) break;

                    parameterIndices[j] = 0;
                    parameterIndices[j + 1]++;
                }
            }

            // Setup table headers
            ColumnHeader[] headers = fieldParameters
                .Select(fp => fp.Field.Name)
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
            while (parameterIndices[^1] != fieldParameterCounts[^1])
            {
                object[] result = new object[table.Columns.Count];

                // Set parameter values and fill in parameter values in table
                for (int i = 0; i < fieldParameters.Length; i++)
                {
                    (FieldInfo field, object[] parameters) = fieldParameters[i];
                    field.SetValue(instance, parameters[parameterIndices[i]]);

                    result[fieldParameters.Length - i - 1] = parameters[parameterIndices[i]];
                }

                // Perform iterations and save results
                double[] scores = new double[iterations];
                long[] times = new long[iterations];
                for (int i = 0; i < iterations; i++)
                {
                    // Run GC to increase consistency
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    long startTime = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
                    scores[i] = instance.RunExperiment();
                    times[i] = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond - startTime;
                }

                // Calculate and set stats in table
                object v1, v2, v3, v4;
                (v1, v2, v3, v4) = CalculateStats(scores);
                result[^8] = v1;
                result[^7] = v2;
                result[^6] = v3;
                result[^5] = v4;
                (v1, v2, v3, v4) = CalculateStats(times);
                result[^4] = v1;
                result[^3] = v2;
                result[^2] = v3;
                result[^1] = v4;

                results.Add(result);
                if (useFile) WriteResult(filename, result);

                // Count up parameter indices
                parameterIndices[0]++;
                for (int i = 0; i < parameterIndices.Length - 1; i++)
                {
                    if (parameterIndices[i] != fieldParameterCounts[i]) break;

                    parameterIndices[i] = 0;
                    parameterIndices[i + 1]++;
                }
            }

            table.AddRows(results);

            // Print out table
            Console.WriteLine(table.ToString());
        }

        public static void RunOptimisation<T>(T instance, int iterations, int optimisationIterations)
            where T : Experiment
        {
            Random rnd = new Random();

            // Get fields with a ParametersAttribute and the parameters for each of them
            (FieldInfo Field, object[] Parameters)[] fieldParameters = GetFieldParameters(instance.GetType());

            foreach ((FieldInfo field, object[] parameters) in fieldParameters)
                if (!parameters.Contains(field.GetValue(instance)))
                    field.SetValue(instance, parameters[parameters.Length / 2]);

            // For number of iterations
            for (int i = 0; i < optimisationIterations; i++)
            {
                // For each field
                foreach ((FieldInfo field, object[] parameters) in fieldParameters.OrderBy(_ => rnd.Next()))
                {
                    // For each parameter get the average score with that parameter set
                    List<double> scores = new List<double>();
                    foreach (object parameter in parameters)
                    {
                        field.SetValue(instance, parameter);

                        scores.Add(CalculateScore(instance, iterations, fieldParameters));
                    }

                    // Find best score
                    int bestIndex = 0;
                    double bestScore = scores[0];
                    for (int index = 0; index < scores.Count; index++)
                    {
                        double score = scores[index];
                        if (!(score < bestScore)) continue;
                        bestScore = score;
                        bestIndex = index;
                    }

                    // Set field to parameter that gave the best score
                    field.SetValue(instance, parameters[bestIndex]);

                    Console.WriteLine(
                        $"Optimising {field.Name} gave a best score of {scores[bestIndex].ToString(CultureInfo.CurrentCulture)} for the parameter {parameters[bestIndex]}");
                }

                Console.WriteLine($"After {i + 1} iterations the configuration is:");
                foreach ((FieldInfo field, object[] _) in fieldParameters)
                    Console.WriteLine($"\t{field.Name}: {field.GetValue(instance)}");
            }

            // Calculates the score by running a number of iterations with the current configuration or by looking up
            // the result of a previous run with the same configuration
            static double CalculateScore(T instance, int iterations,
                IEnumerable<(FieldInfo Field, object[] Parameters)> fieldParameters)
            {
                List<object> key = fieldParameters.Select(tuple => tuple.Field)
                    .Select(field => field.GetValue(instance))
                    .ToListKey();

                if (instance.ScoresDictionary.ContainsKey(key))
                    return instance.ScoresDictionary[key];

                // Perform a number of runs based on the number of iterations and store the results
                List<double> scores = new List<double>();
                for (int i = 0; i < iterations; i++) scores.Add(instance.RunExperiment());

                double score = CalculateStats(scores).Average;
                instance.ScoresDictionary[key] = score;

                // Return average
                return score;
            }
        }

        /// <summary>
        ///     Get all the fields with a <code>ParametersAttribute</code> and the parameters for those fields.
        /// </summary>
        /// <param name="type">The type to get the fields and parameters from.</param>
        /// <returns>An array to tuples containing pairs of field and parameters for that field</returns>
        /// <exception cref="ParameterTypeMismatchException">
        ///     A parameter has a type that does not match the type of the field.
        /// </exception>
        private static (FieldInfo Field, object[] Parameters)[] GetFieldParameters(Type type)
        {
            (FieldInfo Field, object[] Parameters)[] fieldParameters = type.GetFields()
                .Where(field => // Restrict to only fields with ParametersAttribute
                {
                    ParametersAttribute parametersAttribute = field.GetCustomAttribute<ParametersAttribute>();
                    if (parametersAttribute == null || parametersAttribute.Parameters.Length == 0) return false;

                    // Check all parameters are of the correct type
                    if (parametersAttribute.Parameters.Any(p =>
                        p.GetType() != field.FieldType || p.GetType().IsSubclassOf(field.GetType())))
                        throw new ParameterTypeMismatchException(
                            $"The field {field.Name} as a parameter of the wrong type.");

                    return true;
                }).OrderBy(field =>
                {
                    ParametersAttribute parametersAttribute = field.GetCustomAttribute<ParametersAttribute>();
                    return (parametersAttribute.Priority, field.Name);
                })
                .Select(field => (field, field.GetCustomAttribute<ParametersAttribute>().Parameters))
                .ToArray();
            return fieldParameters;
        }

        /// <summary>
        ///     Calculate the average, min, max and standard deviation of an array of values.
        /// </summary>
        /// <param name="values">The array of values to calculate the stats with.</param>
        /// <returns>A tuple containing the average, min, max and standard deviation of the values.</returns>
        private static (double Average, double Min, double Max, double SD) CalculateStats(
            IReadOnlyCollection<double> values)
        {
            double min = values.Min(l => l);
            double max = values.Max(l => l);
            double average = values.Average();
            double sd = Math.Sqrt(1d / values.Count * values.Sum(l => Math.Pow(l - average, 2)));

            return (average, min, max, sd);
        }

        /// <summary>
        ///     Calculate the average, min, max and standard deviation of an array of values.
        /// </summary>
        /// <param name="values">The array of values to calculate the stats with.</param>
        /// <returns>A tuple containing the average, min, max and standard deviation of the values.</returns>
        private static (double, long, long, double) CalculateStats(IReadOnlyCollection<long> values)
        {
            long min = values.Min(l => l);
            long max = values.Max(l => l);
            double average = values.Average();
            double sd = Math.Sqrt(1d / values.Count * values.Sum(l => Math.Pow(l - average, 2)));

            return (average, min, max, sd);
        }

        /// <summary>
        ///     Write a result to the a file by appending it to the end. If the files does not exist wit will be
        ///     created.
        /// </summary>
        /// <param name="filename">The name of the file to write result to.</param>
        /// <param name="result">The result as an array of objects.</param>
        private static void WriteResult(string filename, object[] result)
        {
            // If there is no file create new file with line and be done
            if (!File.Exists(filename))
            {
                File.AppendAllLines(filename, new[] {string.Join('\t', result)});
                return;
            }

            string filenameBackup = filename + ".bak";
            // Create backup
            File.Copy(filename, filenameBackup);
            // Modify backup
            File.AppendAllLines(filenameBackup, new[] {string.Join('\t', result)});
            // Delete original
            File.Delete(filename);
            // Make backup original
            File.Move(filenameBackup, filename);
        }

        /// <summary>
        ///     Load results from a file.
        /// </summary>
        /// <param name="filename">The name of the file to load the results from.</param>
        /// <returns>A list of all the results loaded from the file.</returns>
        private static List<object[]> LoadResults(string filename)
        {
            string filenameBackup = filename + ".bak";

            // Check if file exists
            if (File.Exists(filename) && File.Exists(filenameBackup))
                File.Delete(filenameBackup);
            // Check if backup exists
            else if (File.Exists(filenameBackup)) File.Move(filenameBackup, filename);

            // If a file was found load from it
            if (File.Exists(filename))
                return File.ReadLines(filename)
                    .Select(line => line.Split('\t').Select(s => (object) s).ToArray())
                    .ToList();

            // Return empty list
            return new List<object[]>();
        }

        /// <summary>
        ///     Creates a unique filename based on a list of fields and parameters.
        /// </summary>
        /// <param name="fieldParameters">The list of fields and parameters to create file name from.</param>
        /// <returns>A unique filename.</returns>
        private static string GetFileName(IEnumerable<(FieldInfo Field, object[] Parameters)> fieldParameters)
        {
            // Create unique string from fields and parameters
            string uniqueString = fieldParameters.Aggregate("",
                (current1, fieldParameter) => current1 + fieldParameter.Field.Name + "\0" +
                                              fieldParameter.Field.FieldType.Name + "\0" +
                                              fieldParameter.Parameters.Aggregate(current1,
                                                  (current, parameter) => current + (parameter + "\0")));

            // Compute a hash from the unique string
            long hash = 45345;
            for (int i = 0; i < uniqueString.Length; i += 8)
            {
                string s = uniqueString[new Range(i, Math.Min(i + 8, uniqueString.Length))];
                byte[] bytes = Encoding.UTF8.GetBytes(s, 0, s.Length);
                bytes = bytes.Concat(new byte[8 - bytes.Length]).ToArray();
                long int64 = BitConverter.ToInt64(bytes);
                hash ^= int64;
            }

            return hash + ".results";
        }
    }
}