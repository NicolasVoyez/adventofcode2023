using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AdventOfCode2023.Solvers
{
    class SolverDay4 : ISolver
    {
        List<HashSet<int>> _winning = new List<HashSet<int>>();
        List<HashSet<int>> _numbers = new List<HashSet<int>>();
        Dictionary<int,List<int>> _matches = new Dictionary<int, List<int>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                var split = currentLine.Split(new char[] { ':', '|' });
                _winning.Add(split[1].SplitREE().Select(int.Parse).ToHashSet());
                _numbers.Add(split[2].SplitREE().Select(int.Parse).ToHashSet());
            }
        }

        public string SolveFirstProblem()
        {
            int sum = 0;
            for (int i = 0; i < _winning.Count; i++)
            {
                _matches[i + 1] = new List<int>();
                var curr = 0;
                foreach (var winner in _winning[i])
                {
                    if (_numbers[i].Contains(winner))
                    {
                        if (curr == 0)
                            curr = 1;
                        else 
                            curr *= 2;
                        _matches[i + 1].Add(winner);
                    }
                }
                sum += curr;
            }
            return sum.ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            int[] count = new int[_matches.Count+1];
            int[] toTreat = new int[_matches.Count + 1];
            for (int i = 1; i <= _matches.Count; i++)
            {
                count[i] = 1;
                toTreat[i] = 1;
            }


            foreach(var (i, matches) in _matches)
            {
                var curr = toTreat[i];
                if (curr == 0)
                    continue;

                for (int j = 1; j < matches.Count + 1; j++)
                {
                    toTreat[i+j] += curr;
                    count[i+j] += curr;
                }
                toTreat[i] = 0;
            }

                /* if the exercice was interesting
                while (toTreat.Any(t => t!= 0))
                {
                    for (int i = 1; i < _matches.Count; i++)
                    {
                        var curr = toTreat[i];
                        if (curr == 0)
                            continue;
                        foreach(var card in _matches[i])
                        {
                            toTreat[i] += card * curr;
                            count[i] += card * curr;
                        }
                        toTreat[i] = 0;
                    }
                }
                */
                return count.Sum().ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
