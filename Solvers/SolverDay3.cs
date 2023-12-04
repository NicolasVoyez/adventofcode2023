using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

namespace AdventOfCode2023.Solvers
{
    class SolverDay3 : ISolver
    {
        Grid<char> _grid;
        List<(int, Grid<char>.Cell<char>)> _numbers;
        public void InitInput(string content)
        {
            _grid = new Grid<char>(content, c => c);
            _numbers = new List<(int, Grid<char>.Cell<char>)>();
            for (int y = 0; y < _grid.YMax; y++)
            {
                string currentNum = "";
                Grid<char>.Cell<char> symbol = default;
                for (int x = 0; x < _grid.XMax; x++)
                {
                    var curr = _grid[y, x];
                    if (char.IsDigit(curr.Value))
                    {
                        currentNum += curr.Value;
                        if (symbol == default)
                        {
                            foreach (var around in _grid.Around(curr))
                            {
                                if (!char.IsDigit(around.Value) && around.Value != '.')
                                {
                                    symbol = around;
                                    break;
                                }
                            }
                        }
                    }
                    else if (currentNum != "")
                    {
                        if (symbol != default)
                        {
                            _numbers.Add((int.Parse(currentNum), symbol));
                        }
                        currentNum = "";
                        symbol = default;
                    }
                }
                if (symbol != default)
                {
                    _numbers.Add((int.Parse(currentNum), symbol));
                }
            }
        }

        public string SolveFirstProblem()
        {
            return _numbers.Sum(n => n.Item1).ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            return _numbers.GroupBy(x => x.Item2).Where(g => g.Count() == 2 && g.Key.Value == '*').Sum(g => g.First().Item1 * g.Last().Item1).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
