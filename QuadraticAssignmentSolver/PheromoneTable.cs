// CameronSalisbury_1293897

using System;
using System.Linq;

namespace QuadraticAssignmentSolver
{
    public class PheromoneTable
    {
        /// <summary>
        ///     The problem being solved.
        /// </summary>
        private readonly Problem _problem;

        /// <summary>
        ///     The table of pheromones.
        /// </summary>
        private readonly double[] _table;

        /// <summary>
        ///     Indicates if the table has been initialised yet.
        /// </summary>
        private bool _initialised;

        /// <summary>
        ///     The maximum possible pheromone value.
        /// </summary>
        private double _max;

        /// <summary>
        ///     The minimum possible pheromone value.
        /// </summary>
        private double _min;

        /// <summary>
        ///     The portion of pheromone to be carried over in an update.
        /// </summary>
        public double EvaporationRate;

        /// <summary>
        ///     The approximate probability of generating best know solution if pheromone table has converged and is used to
        ///     determine what the minimum possible pheromone value should be.
        /// </summary>
        public double ProbBest;

        public PheromoneTable(Problem problem, double evaporationRate, double probBest)
        {
            _problem = problem;

            // Set all values to 0 to indicate pheromones have not been initialised yet, they will be initialised when
            // first pheromones are deposited
            _table = Enumerable.Repeat(1d, _problem.Size * _problem.Size).ToArray();

            EvaporationRate = evaporationRate;
            ProbBest = probBest;
        }

        /// <summary>
        ///     Get the pheromone level for a location and facility.
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
        ///     Update the pheromone values based on provided solutions.
        /// </summary>
        /// <param name="solution">The solution to use to update pheromones.</param>
        public void DepositPheromones(Solution solution)
        {
            // If this is first update set all values to max
            if (!_initialised)
            {
                Array.Fill(_table, _max);
                _initialised = true;
            }

            // For each location and facility calculate new pheromone level
            for (int location = 0; location < _problem.Size; location++)
            {
                int facilityAtLocation = solution.GetFacility(location);
                for (int facility = 0; facility < _problem.Size; facility++)
                {
                    // Update pheromones
                    _table[location * _problem.Size + facility] =
                        EvaporationRate * _table[location * _problem.Size + facility]
                        + (facilityAtLocation == facility ? 1d / solution.Fitness : 0);

                    // Clamp between min and max
                    _table[location * _problem.Size + facility] =
                        Math.Clamp(_table[location * _problem.Size + facility], _min, _max);
                }
            }
        }

        /// <summary>
        ///     Update the maximum and minimum possible pheromone values based on best known solution.
        /// </summary>
        /// <param name="best">The current best known solution.</param>
        public void UpdateMaxAndMin(Solution best)
        {
            // Set max as the asymptote that would be reached if the best solution were to be deposited repeatedly
            _max = 1d / (1d - EvaporationRate) * (1d / best.Fitness);

            // Set the min such that an ant would have ProbBest chance of generating the best solution if the search has
            // converged
            double pDec = Math.Pow(ProbBest, 1d / best.Size);
            _min = _max * (1 - pDec) / ((best.Size / 2d - 1) * pDec);

            // Check min is less than max
            if (_min > _max) _min = _max;
        }

        /// <summary>
        ///     Get the maximum and minimum possible pheromone values.
        /// </summary>
        /// <returns>The maximum and minimum possible pheromone values.</returns>
        public (double Max, double Min) GetMaxAndMin()
        {
            return (_max, _min);
        }
    }
}