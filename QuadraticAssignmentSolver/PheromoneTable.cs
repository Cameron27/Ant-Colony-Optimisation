using System;
using System.Collections.Generic;
using System.Linq;

namespace QuadraticAssignmentSolver
{
    public class PheromoneTable
    {
        /// <summary>
        ///     The initial pheromone value to use.
        /// </summary>
        public static double InitialValue = 0.1;

        /// <summary>
        ///     The portion of pheromone to be carried over in an update.
        /// </summary>
        public static double EvaporationRate = 0.5;

        /// <summary>
        ///     The problem being solved.
        /// </summary>
        private readonly Problem _problem;

        /// <summary>
        ///     The table of pheromones.
        /// </summary>
        private readonly double[] _table;

        public PheromoneTable(Problem problem)
        {
            _problem = problem;
            _table = Enumerable.Repeat(InitialValue, _problem.Size * _problem.Size).ToArray();
        }

        /// <summary>
        /// Get the pheromone level for a location and facility.
        /// </summary>
        /// <param name="location">The location to lookup the pheromone for.</param>
        /// <param name="facility">The facility to lookup the pheromone for.</param>
        /// <returns>The pheromone level for a location and facility.</returns>
        public double GetPheromones(int location, int facility)
        {
            return _table[location * _problem.Size + facility];
        }
        
        public double[] GetAllPheromones()
        {
            return _table;
        }
        
        public void SetAllPheromones(double[] pheromones)
        {
            Array.Copy(pheromones, _table, pheromones.Length);
        }

        /// <summary>
        /// Update the pheromone values based on provided solutions.
        /// </summary>
        /// <param name="solutions">The solutions to use to update pheromones.</param>
        public void DepositPheromones(IEnumerable<Solution> solutions)
        {
            // Calculate the amount to be deposited at each location
            double[] depositAmounts = CalculateDepositAmounts(solutions);

            // For each location and facility calculate new pheromone level
            for (int location = 0; location < _problem.Size; location++)
            for (int facility = 0; facility < _problem.Size; facility++)
                _table[location * _problem.Size + facility] =
                    EvaporationRate * _table[location * _problem.Size + facility]
                    + depositAmounts[location * _problem.Size + facility];
        }

        /// <summary>
        /// Calculate the amount of pheromones to deposit at each location based on provided solutions. 
        /// </summary>
        /// <param name="solutions">The solutions to use to calculate the amount of pheromones to deposit.</param>
        /// <returns>The amount of pheromones to deposit at each location.</returns>
        private double[] CalculateDepositAmounts(IEnumerable<Solution> solutions)
        {
            double[] depositAmounts = new double[_problem.Size * _problem.Size];

            // For each solution
            foreach (Solution solution in solutions)
            {
                // Calculate pheromone amount based on quality of solution 
                double pheromoneAmount = 1d / solution.Fitness;

                // For each location add pheromone amount based on the facility at that location
                for (int location = 0; location < _problem.Size; location++)
                {
                    int facility = solution.GetFacility(location);

                    depositAmounts[location * _problem.Size + facility] += pheromoneAmount;
                }
            }

            return depositAmounts;
        }
    }
}