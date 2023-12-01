using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace AdventOfCode2023.Solvers
{
    class SolverDay1 : ISolver
    {
        private List<string> _enhancedLines = new List<string>();
        Dictionary<string, int> _digitsSpelling = new Dictionary<string, int>
        {
            {"one", 1 }, {"two", 2 }, {"three", 3 }, {"four", 4 }, {"five", 5 }, {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9}
        };
        public void InitInput(string content)
        {
            _enhancedLines = content.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
        }

        public string SolveFirstProblem()
        {
            var firstAndLastDigits = new List<(int, int)>();
            foreach (var line in _enhancedLines)
            {
                int first = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (Char.IsDigit(line[i]))
                    {
                        first = int.Parse(line[i].ToString());
                        break;
                    }
                }
                int last = 0;
                for (int i = line.Length -1; i >= 0; i--)
                {
                    if (Char.IsDigit(line[i]))
                    {
                        last = int.Parse(line[i].ToString());
                        break;
                    }
                }
                firstAndLastDigits.Add((first,last));
            }

            return firstAndLastDigits.Select(x => x.Item1 * 10 + x.Item2).Sum().ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var firstAndLastDigits = new List<(int, int)>();
            foreach (var line in _enhancedLines)
            {
                int first = 0;
                int firstIdx = int.MaxValue;
                int last = 0;
                int lastIdx = int.MinValue;
                foreach (var (text, val) in _digitsSpelling)
                {
                    int f = line.IndexOf(text);
                    if (f != -1)
                    {
                        if (f < firstIdx)
                        {
                            first = val;
                            firstIdx = f;
                        }
                        int l = line.LastIndexOf(text);
                        if (l > lastIdx)
                        {
                            last = val;
                            lastIdx = l;
                        }
                    }
                    for (int i = 0; i < Math.Min(line.Length,firstIdx); i++)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            first = int.Parse(line[i].ToString());
                            break;
                        }
                    }
                    for (int i = line.Length - 1; i >= Math.Max(0,lastIdx); i--)
                    {
                        if (Char.IsDigit(line[i]))
                        {
                            last = int.Parse(line[i].ToString());
                            break;
                        }
                    }
                }
                firstAndLastDigits.Add((first, last));
            }

            return firstAndLastDigits.Select(x => x.Item1 * 10 + x.Item2).Sum().ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
