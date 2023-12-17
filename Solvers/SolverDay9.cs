using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023.Solvers
{
    class SolverDay9 : ISolver
    {
        List<List<BigInteger>> suites = new List<List<BigInteger>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                suites.Add(currentLine.SplitAsBigInteger().ToList());
            }
        }

        public string SolveFirstProblem()
        {
            BigInteger result = 0;
            foreach(var suite in suites)
            {
                result += GetNextL1(suite);
            }
            return result.ToString();
        }

        private List<List<List<BigInteger>>> _allLevels = new List<List<List<BigInteger>>>();
        private BigInteger GetNextL1(List<BigInteger> suite)
        {
            List<List<BigInteger>> levels = new List<List<BigInteger>>() { suite };
            List<BigInteger> lastLevel = suite;
            while (lastLevel.Any(x => x != lastLevel[0])) { 
                var newone = new List<BigInteger> { };
                for (int i = 1; i < lastLevel.Count; i++)
                {
                    newone.Add(lastLevel[i] - lastLevel[i-1]);
                }
                levels.Add(newone);
                lastLevel = newone;
            }
            var curr = lastLevel.Last();
            lastLevel.Add(curr);

            _allLevels.Add(levels);
            for (int l = levels.Count - 2; l >= 0; l--)
            {
                var now = curr + levels[l].Last();
                levels[l].Add(now );
                curr = now;                
            }
            return curr;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            BigInteger sum = 0;
            foreach (var levels in _allLevels)
            {
                var curr = levels.Last()[0];

                for (int l = levels.Count - 2; l >= 0; l--)
                {
                    curr = levels[l].First() - curr;
                }
                sum += curr;

            }
            return sum.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
