using System;
using System.IO;
using System.Linq;
using System.Text;

namespace QuadraticAssignmentSolver
{
    public class Solution
    {
        private readonly Problem _problem;
        private int[] _facilitiesAtLocations;
        private int? _fitness;

        public Solution(Problem problem)
        {
            _problem = problem;

            _facilitiesAtLocations = Enumerable.Repeat(-1, _problem.Size).ToArray();
        }

        public int Size => _problem.Size;

        public int Fitness
        {
            get
            {
                // If fitness is known, return it
                if (_fitness.HasValue) return _fitness.Value;

                // Calculate fitness
                int fitness = 0;
                // For each location
                for (int locationB = 0; locationB < _problem.Size; locationB++)
                {
                    int facilityB = _facilitiesAtLocations[locationB];

                    if (facilityB == -1) continue;

                    // For each location of a higher number
                    for (int locationA = locationB + 1; locationA < _problem.Size; locationA++)
                    {
                        int facilityA = _facilitiesAtLocations[locationA];

                        if (facilityA == -1) continue;

                        // Add (flow * distance) to fitness 
                        fitness += _problem.GetFlow(facilityA, facilityB) * _problem.GetDistance(locationA, locationB);
                    }
                }

                // Double fitness for symmetry
                _fitness = fitness * 2;

                return _fitness.Value;
            }
        }

        public void SetFacility(int locationIndex, int facilityIndex)
        {
#if DEBUG
            // Check validity
            if (facilityIndex >= _problem.Size || facilityIndex < -1)
                throw new IndexOutOfRangeException(
                    $"The facility index, {facilityIndex}, is outside of the range of -1 to {_problem.Size - 1}.");
            if (locationIndex >= _problem.Size || locationIndex < 0)
                throw new IndexOutOfRangeException(
                    $"The location index, {locationIndex}, is outside of the range of 0 to {_problem.Size - 1}.");
#endif

            _facilitiesAtLocations[locationIndex] = facilityIndex;

            // Set fitness to unknown
            _fitness = null;
        }

        public int GetFacility(int locationIndex)
        {
#if DEBUG
            // Check validity
            if (locationIndex >= _problem.Size)
                throw new IndexOutOfRangeException(
                    $"The location index, {locationIndex}, is larger than the size of the problem, {_problem.Size}.");
#endif

            int result = _facilitiesAtLocations[locationIndex];
            if (result == -1) throw new NullReferenceException("Location does not have a facility assigned to it.");
            return result;
        }

        public int PartialFitness(int location)
        {
            int partialFitness = 0;

            int facilityA = _facilitiesAtLocations[location];

            // For every location
            for (int locationB = 0; locationB < _problem.Size; locationB++)
            {
                if (location == locationB) continue;

                int facilityB = _facilitiesAtLocations[locationB];

                if (facilityB == -1) continue;

                // Add (flow * distance) to partial fitness
                partialFitness += _problem.GetFlow(facilityA, facilityB) * _problem.GetDistance(location, locationB);
            }

            // Double for symmetry
            return partialFitness * 2;
        }
        
        public int PartialFitness(in int locationA, in int locationB)
        {
            int facilityA = _facilitiesAtLocations[locationA];
            int facilityB = _facilitiesAtLocations[locationB];
            return _problem.GetFlow(facilityA, facilityB) * _problem.GetDistance(locationA, locationB) * 2;
        }

        public static (Solution, int) CreateFromFile(string filename, Problem problem)
        {
            int[] numbers = File.ReadAllLines(filename) //For each line
                .SelectMany(line => line.Split(' ')) // Split by space and flatten
                .Where(s => s.Length != 0) // Remove empty strings
                .Select(s => // Convert to ints
                {
                    if (int.TryParse(s, out int i)) return i;

                    throw new FormatException("A component of the file is not a number.");
                })
                .ToArray();

            // Get known fitness
            int knownFitness = numbers[1];

            // Create solution
            Solution solution = new Solution(problem);
            for (int i = 0; i < problem.Size; i++) solution.SetFacility(i, numbers[i + 2] - 1);

            return (solution, knownFitness);
        }

        public void DisplayResult()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Problem Size: ").AppendLine(Size.ToString());
            sb.Append("Solution fitness: ").AppendLine(Fitness.ToString());

            sb.Append("Solution: ");
            for (int i = 0; i < Size; i++)
            {
                sb.Append(GetFacility(i) + 1);
                if (i != Size - 1) sb.Append(" ");
            }

            Console.WriteLine(sb.ToString());
        }

        public Solution Clone()
        {
            return new Solution(_problem) {_facilitiesAtLocations = _facilitiesAtLocations.Clone() as int[]};
        }
    }
}