using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuadraticAssignmentSolver
{
    public class Problem
    {
        private readonly int[] _distances;
        private readonly int[] _flows;

        private Problem(int size)
        {
            Size = size;
            int matrixSize = (size * size - size) / 2;
            _distances = new int[matrixSize];
            _flows = new int[matrixSize];
        }

        public int Size { get; }

        public int GetDistance(int a, int b)
        {
            if (a == b) return 0;
            if (a < b) (a, b) = (b, a);

            int i = (int) (b * Size + a - (b + 2) * ((b + 1) / 2F));
            return _distances[i];
        }

        public int GetFlow(int a, int b)
        {
            if (a == b) return 0;
            if (a < b) (a, b) = (b, a);

            int i = (int) (b * Size + a - (b + 2) * ((b + 1) / 2F));
            return _flows[i];
        }

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

            CopySymmetricMatrix(problem._distances, numbers, 1, size);
            CopySymmetricMatrix(problem._flows, numbers, 1 + size * size, size);

            return problem;
        }

        private static void CopySymmetricMatrix(IList<int> destination, IReadOnlyList<int> source, int sourceOffset,
            int size)
        {
            int index = 0;
            int sizeSquared = size * size;
            for (int i = 0; i < sizeSquared; i++)
            {
                int x = i % size;
                int y = i / size;

                if (y >= x) continue;

                destination[index] = source[i + sourceOffset];

                index++;
            }
        }
    }
}