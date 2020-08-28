using System;
using System.Text;

namespace QuadraticAssignmentSolver.Utils
{
    public static class Utils
    {
        public static void DisplayResult(this (Solution, int) s)
        {
            (Solution solution, int fitness) = s;

            StringBuilder sb = new StringBuilder();

            sb.Append("Problem Size: ").AppendLine(solution.Size.ToString());
            sb.Append("Solution fitness: ").AppendLine(fitness.ToString());

            sb.Append("Solution: ");
            for (int i = 0; i < solution.Size; i++)
            {
                sb.Append(solution.GetFacility(i) + 1);
                if (i != solution.Size - 1) sb.Append(" ");
            }

            sb.Append(Environment.NewLine);

            Console.WriteLine(sb.ToString());
        }
    }
}