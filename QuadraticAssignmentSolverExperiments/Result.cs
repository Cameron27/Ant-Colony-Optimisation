namespace QuadraticAssignmentSolver.Experiments
{
    public readonly struct Result
    {
        public readonly int Fitness;
        public readonly long Time;

        public Result(int fitness, long time)
        {
            Fitness = fitness;
            Time = time;
        }

        public override string ToString()
        {
            return $"({Fitness}, {Time})";
        }
    }
}