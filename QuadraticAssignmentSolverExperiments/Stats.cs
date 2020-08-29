using System;
using System.Collections.Generic;
using System.Linq;

namespace QuadraticAssignmentSolver.Experiments
{
    public readonly struct Stats
    {
        public readonly int FitnessMin;
        public readonly int FitnessMax;
        public readonly double FitnessAverage;
        public readonly double FitnessStandardDeviation;
        public readonly long TimeMin;
        public readonly long TimeMax;
        public readonly double TimeAverage;
        public readonly double TimeStandardDeviation;

        public Stats(Result[] results) : this()
        {
            (FitnessMin, FitnessMax, FitnessAverage, FitnessStandardDeviation) =
                CalculateStats(results, r => r.Fitness, l => (int) l);
            (TimeMin, TimeMax, TimeAverage, TimeStandardDeviation) = CalculateStats(results, r => r.Time, l => l);
        }

        private static (T, T, double, double) CalculateStats<T>(IEnumerable<Result> results, Func<Result, long> f,
            Func<long, T> g)
        {
            long[] values = results.Select(f).ToArray();

            T min = g(values.Min(l => l));
            T max = g(values.Max(l => l));
            double average = values.Average();
            double sd = Math.Sqrt(1d / values.Length * values.Sum(l => Math.Pow(l - average, 2)));

            return (min, max, average, sd);
        }

        public override string ToString()
        {
            return
                $"({FitnessMin}, {FitnessMax}, {FitnessAverage}, {FitnessStandardDeviation}, {TimeMin}, {TimeMax}, {TimeAverage}, {TimeStandardDeviation})";
        }
    }
}