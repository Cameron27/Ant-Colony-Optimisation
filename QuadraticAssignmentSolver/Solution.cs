using System;
using System.IO;
using System.Linq;

namespace QuadraticAssignmentSolver
{
    public class Solution
    {
        private readonly Problem _problem;
        private int[] _facilitiesAtLocations;

        public Solution(Problem problem)
        {
            _problem = problem;

            _facilitiesAtLocations = Enumerable.Repeat(-1, _problem.Size).ToArray();
        }

        public int Size => _problem.Size;

        public void SetFacility(int locationIndex, int facilityIndex)
        {
            if (facilityIndex >= _problem.Size)
                throw new IndexOutOfRangeException(
                    $"The facility index, {facilityIndex}, is larger than the size of the problem, {_problem.Size}.");
            if (locationIndex >= _problem.Size)
                throw new IndexOutOfRangeException(
                    $"The location index, {locationIndex}, is larger than the size of the problem, {_problem.Size}.");

            _facilitiesAtLocations[locationIndex] = facilityIndex;
        }

        public int GetFacility(int locationIndex)
        {
            if (locationIndex >= _problem.Size)
                throw new IndexOutOfRangeException(
                    $"The location index, {locationIndex}, is larger than the size of the problem, {_problem.Size}.");

            return _facilitiesAtLocations[locationIndex];
        }

        public int EvaluateFitness()
        {
            int fitness = 0;
            for (int locationB = 0; locationB < _problem.Size; locationB++)
            {
                int facilityB = _facilitiesAtLocations[locationB];

                if (facilityB == -1) continue;

                for (int locationA = locationB + 1; locationA < _problem.Size; locationA++)
                {
                    int facilityA = _facilitiesAtLocations[locationA];

                    if (facilityA == -1) continue;

                    fitness += _problem.GetFlow(facilityA, facilityB) * _problem.GetDistance(locationA, locationB);
                }
            }

            return fitness * 2;
        }

        public int EvaluatePartialFitness(int location)
        {
            int partialFitness = 0;

            int facilityA = _facilitiesAtLocations[location];

            for (int locationB = 0; locationB < _problem.Size; locationB++)
            {
                if (location == locationB) continue;

                int facilityB = _facilitiesAtLocations[locationB];

                if (facilityB == -1) continue;

                partialFitness += _problem.GetFlow(facilityA, facilityB) * _problem.GetDistance(location, locationB);
            }

            return partialFitness * 2;
        }

        public static (Solution, int) CreateFromFile(string filename, Problem problem)
        {
            int[] numbers = File.ReadAllLines(filename)
                .SelectMany(line => line.Split(' '))
                .Where(s => s.Length != 0)
                .Select(s =>
                {
                    if (int.TryParse(s, out int i)) return i;

                    throw new FormatException("A component of the file is not a number.");
                })
                .ToArray();

            int knownFitness = numbers[1];
            Solution solution = new Solution(problem);

            for (int i = 0; i < problem.Size; i++) solution.SetFacility(i, numbers[i + 2] - 1);

            return (solution, knownFitness);
        }

        public Solution Clone()
        {
            return new Solution(_problem) { _facilitiesAtLocations = _facilitiesAtLocations.Clone() as int[] };
        }
    }
}