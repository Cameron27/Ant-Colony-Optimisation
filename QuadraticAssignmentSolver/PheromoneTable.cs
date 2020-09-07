using System.Collections.Generic;
using System.Linq;

namespace QuadraticAssignmentSolver
{
    public class PheromoneTable
    {
        public static double InitialValue = 0.5;
        public static double EvaporationRate = 0.5;
        private readonly Problem _problem;

        private readonly double[] _table;

        public PheromoneTable(Problem problem)
        {
            _problem = problem;
            _table = Enumerable.Repeat(InitialValue, _problem.Size * _problem.Size).ToArray();
        }

        public double GetPheromones(int locationIndex, int facilityIndex)
        {
            return _table[locationIndex * _problem.Size + facilityIndex];
        }

        public void DepositPheromones(IEnumerable<Solution> solutions)
        {
            double[] depositAmounts = CalculateDepositAmounts(solutions);

            for (int location = 0; location < _problem.Size; location++)
            for (int facility = 0; facility < _problem.Size; facility++)
                _table[location * _problem.Size + facility] =
                    EvaporationRate * _table[location * _problem.Size + facility]
                    + depositAmounts[location * _problem.Size + facility];
        }

        private double[] CalculateDepositAmounts(IEnumerable<Solution> solutions)
        {
            double[] depositAmounts = new double[_problem.Size * _problem.Size];

            foreach (Solution solution in solutions)
            {
                double weight = CalculateWeight(solution.Fitness);

                for (int location = 0; location < _problem.Size; location++)
                {
                    int facility = solution.GetFacility(location);

                    depositAmounts[location * _problem.Size + facility] += weight;
                }
            }

            return depositAmounts;
        }

        private static double CalculateWeight(int score)
        {
            return 1d / score;
        }
    }
}