using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023.Solvers
{
    class SolverDay11 : ISolver
    {
        private List<List<char>> _grid;
        private List<Point> _galaxies = new List<Point>();
        private List<BigPoint> _galaxiesEx2 = new List<BigPoint>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            _grid = new List<List<char>> { };
            foreach (var currentLine in splitContent)
            {
                // vertical expand
                if (currentLine.All(c => c == '.'))
                {
                    _grid.Add(new string('B', currentLine.Length).ToList());
                }
                else
                    _grid.Add(currentLine.ToList());
            }
            // expand horizontal
            for (int x = 0; x < _grid[0].Count; x++)
            {
                if (_grid.All(l => l[x] == '.' || l[x] == 'B'))
                {
                    _grid.ForEach(l => l[x] = 'B');
                    x++;
                }
            }

            BigInteger y2 = 0;
            int y1 = 0;
            for (int y = 0; y < _grid.Count; y++)
            {
                var line = _grid[y];
                if (line.All(c => c == 'B'))
                {
                    y2 += 1000000;
                    y1 += 2;
                    continue;
                }
                BigInteger x2 = 0;
                int x1 = 0;
                for (int x = 0; x < line.Count; x++)
                {

                    if (_grid.All(l => l[x] == 'B'))
                    {
                        x2 += 1000000;
                        x1 += 2;
                        continue;
                    }
                    var current = line[x];
                    if (current == '#')
                    {
                        _galaxies.Add(new Point(y1, x1));
                        _galaxiesEx2.Add(new BigPoint(y2, x2));
                    }
                    x2 += 1;
                    x1 += 1;
                }
                y2 += 1;
                y1 += 1;
            }

            /*
            Console.WriteLine();
            for (int y = 0; y < _grid.Count; y++)
            {
                var line = _grid[y];
                for (int x = 0; x < line.Count; x++)
                {
                    Console.Write(line[x]);
                }
                Console.WriteLine();
            }*/
        }

        public string SolveFirstProblem()
        {
            BigInteger sum = 0;
            int pairs = 0;
            for (int i1 = 0; i1 < _galaxies.Count; i1++)
            {
                for (int i2 = i1 + 1; i2 < _galaxies.Count; i2++)
                {
                    var galaxy1 = _galaxies[i1];
                    var galaxy2 = _galaxies[i2];
                    if (galaxy1 == galaxy2)
                        continue;

                    pairs++;
                    sum += galaxy1.ManhattanDistance(galaxy2);
                }
            }

            return sum.ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            BigInteger sum = 0;
            int pairs = 0;
            for (int i1 = 0; i1 < _galaxiesEx2.Count; i1++)
            {
                for (int i2 = i1 + 1; i2 < _galaxiesEx2.Count; i2++)
                {
                    var galaxy1 = _galaxiesEx2[i1];
                    var galaxy2 = _galaxiesEx2[i2];
                    if (galaxy1 == galaxy2)
                        continue;

                    pairs++;
                    sum += galaxy1.ManhattanDistance(galaxy2);
                }
            }

            return sum.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
