using System;
using System.IO;
using System.Linq;

namespace QuadraticAssignmentSolver
{
    public class Problem
    {
        /// <summary>
        ///     The array of distances between locations.
        /// </summary>
        private readonly int[] _distances;

        /// <summary>
        ///     The array of flows between facilities.
        /// </summary>
        private readonly int[] _flows;

        private Problem(int size)
        {
            Size = size;
            int matrixSize = size * size;
            _distances = new int[matrixSize];
            _flows = new int[matrixSize];
        }

        /// <summary>
        ///     The number of facilities/locations in the problem.
        /// </summary>
        public int Size { get; }

        /// <summary>
        ///     Get the distance between two locations.
        /// </summary>
        /// <param name="a">The first location.</param>
        /// <param name="b">The second location.</param>
        /// <returns>The distance between the two locations.</returns>
        public int GetDistance(int a, int b)
        {
            return _distances[b * Size + a];
        }

        /// <summary>
        ///     Get the flow between two facilities.
        /// </summary>
        /// <param name="a">The first facility.</param>
        /// <param name="b">The second facility.</param>
        /// <returns>The distance between the two facilities.</returns>
        public int GetFlow(int a, int b)
        {
            return _flows[b * Size + a];
        }

        /// <summary>
        ///     Load a problem from a file.
        /// </summary>
        /// <param name="filename">The file name to load the problem from.</param>
        /// <returns>The problems loaded from the file.</returns>
        /// <exception cref="FormatException">The format of the file is not a valid problem.</exception>
        public static Problem CreateFromFile(string filename)
        {
            int[] numbers = File.ReadAllLines(filename)
                .SelectMany(line => line.Split(' '))
                .Where(s => s.Length != 0)
                .Select(s =>
                {
                    if (int.TryParse(s, out int i)) return i;

                    throw new FormatException("A value in the file is not a number.");
                })
                .ToArray();

            if (numbers.Length == 0)
                throw new FormatException("File contains no values.");
            int size = numbers[0];
            Problem problem = new Problem(size);

            if (numbers.Length != 1 + size * size * 2)
                throw new FormatException(
                    $"File contains {numbers.Length} values, {1 + size * size * 2} values were expected for a problem of size {size}.");

            Array.Copy(numbers, 1, problem._distances, 0,
                size * size);
            Array.Copy(numbers, 1 + size * size, problem._flows, 0,
                size * size);

            return problem;
        }
    }
}