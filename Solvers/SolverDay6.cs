using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023.Solvers
{
    class SolverDay6 : ISolver
    {
        private IList<int> _times;
        private IList<int> _distances;
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            _times = splitContent[0].Replace("Time:", "").SplitAsInt();
            _distances = splitContent[1].Replace("Distance:", "").SplitAsInt();
        }

        public string SolveFirstProblem()
        {
            List<int> workingResults = new List<int>();

            for (int race = 0; race < _times.Count; race++)
            {
                int time = _times[race];
                int distance = _distances[race];

                int countWorking = CountWinningWays(time, distance);

                workingResults.Add(countWorking);
            }
            return workingResults.Aggregate(BigInteger.One, (x, y) => x * y).ToString();
        }

        private static int CountWinningWays(BigInteger time, BigInteger distance)
        {
            int countWorking = 0;

            for (int speed = 1; speed < time; speed++)
            {
                if ((time - speed) * speed > distance)
                    countWorking++;
                else if (countWorking > 0)
                    break;
            }

            return countWorking;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var time = int.Parse(_times.Aggregate("", (x, y) => x + y));
            var distance = BigInteger.Parse(_distances.Aggregate("", (x, y) => x + y));

            return CountWinningWays(time, distance).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
