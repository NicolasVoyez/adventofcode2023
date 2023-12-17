using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AdventOfCode2023.Solvers
{
    class SolverDay13 : ISolver
    {
        private List<Grid<char>> _grids = new List<Grid<char>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);

            foreach (var gridstring in splitContent)
            {
                _grids.Add(new Grid<char>(gridstring, c => c));
            }
        }

        private List<(char, int)> _ex1SymetryAxies = new List<(char, int)>();
        public string SolveFirstProblem()
        {
            foreach (var grid in _grids)
            {
                if (TryFindVerticalAxis(grid, -1, out int lowerBound))
                {
                    _ex1SymetryAxies.Add(('V', lowerBound));
                    continue;
                }
                if (TryFindHorizontalAxis(grid, -1, out lowerBound))
                {
                    _ex1SymetryAxies.Add(('H', lowerBound));
                    continue;
                }

                grid.Print((char c) => Console.Write(c));
                throw new NotImplementedException();
            }

            return CalculateResultInt(_ex1SymetryAxies);
        }

        private string CalculateResultInt(List<(char, int)> axies)
        {
            int result = 0;
            for (int i = 0; i < _grids.Count; i++)
            {
                var grid = _grids[i];
                var (dir, lowerBound) = axies[i];
                if (dir == 'V')
                {
                    result += lowerBound + 1;
                }
                else
                {
                    result += (lowerBound + 1) * 100;
                }
            }
            return result.ToString();
        }

        private bool TryFindHorizontalAxis(Grid<char> grid, int skip, out int lowerBound)
        {
            lowerBound = -1;
            for (int sy = 1; sy < grid.YMax; sy++)
            {
                if (sy == skip + 1)
                    continue;
                var row1 = new string(grid.GetRowValues(sy - 1).ToArray());
                var row2 = new string(grid.GetRowValues(sy).ToArray());
                if (row1 == row2)
                {
                    // validate sxmetrx
                    bool valid = true;
                    for (int y = 1; sy - y - 1 >= 0 && sy + y < grid.YMax; y++)
                    {
                        var row1Check = new string(grid.GetRowValues(sy - y - 1).ToArray());
                        var row2Check = new string(grid.GetRowValues(sy + y).ToArray());
                        if (row1Check != row2Check)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        lowerBound = sy - 1;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryFindVerticalAxis(Grid<char> grid, int skip, out int lowerBound)
        {
            lowerBound = -1;
            for (int sx = 1; sx < grid.XMax; sx++)
            {
                if (sx == skip + 1)
                    continue;
                var col1 = new string(grid.GetColumnValues(sx - 1).ToArray());
                var col2 = new string(grid.GetColumnValues(sx).ToArray());
                if (col1 == col2)
                {
                    // validate symetry
                    bool valid = true;
                    for (int x = 1; sx - x - 1 >= 0 && sx + x < grid.XMax; x++)
                    {
                        var col1Check = new string(grid.GetColumnValues(sx - x - 1).ToArray());
                        var col2Check = new string(grid.GetColumnValues(sx + x).ToArray());
                        if (col1Check != col2Check)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        lowerBound = sx - 1;
                        return true;
                    }
                }
            }
            return false;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var axies = new List<(char, int)>();
            for (int i = 0; i < _grids.Count; i++)
            {
                var grid = _grids[i];
                var (dir, lowerBound) = _ex1SymetryAxies[i];
                var vSkip = dir == 'V' ? lowerBound : -1;
                var hSkip = dir == 'H' ? lowerBound : -1;
                foreach (var cell in grid.All())
                {
                    grid.Set(cell.Y, cell.X, cell.Value == '.' ? '#' : '.');

                    if (TryFindVerticalAxis(grid, vSkip, out lowerBound))
                    {
                        axies.Add(('V', lowerBound));
                        break;
                    }
                    if (TryFindHorizontalAxis(grid, hSkip, out lowerBound))
                    {
                        axies.Add(('H', lowerBound));
                        break;
                    }
                    grid.Set(cell.Y, cell.X, cell.Value);
                }
            }

            return CalculateResultInt(axies);
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
