using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers
{
    class SolverDay16 : ISolver
    {
        private Grid<char> _grid;
        private string _content;
        public void InitInput(string content)
        {
            _grid = new Grid<char>(content, c => c);
            _content = content;
        }

        public string SolveFirstProblem()
        {
            var start = (_grid[0, 0], Direction.Right);
            int lighten = CalculateLight(start);
            return lighten.ToString();
        }

        private int CalculateLight((Grid<char>.Cell<char>, Direction Right) start)
        {
            Grid<bool> _lightenGrid = new Grid<bool>(_content, c => false);
            HashSet<(Grid<char>.Cell<char>?, Direction)> done = new HashSet<(Grid<char>.Cell<char>?, Direction)>();
            Stack<(Grid<char>.Cell<char>?, Direction)> todo = new Stack<(Grid<char>.Cell<char>?, Direction)>();
            todo.Push(start);
            while (todo.Count > 0)
            {
                var current = todo.Pop();
                if (!current.Item1.HasValue) continue;
                if (done.Contains(current)) continue;
                done.Add(current);

                var dir = current.Item2;
                var cell = current.Item1.Value;
                _lightenGrid.Set(cell.Y, cell.X, true);

                switch (cell.Value)
                {
                    case '.':
                        todo.Push((_grid.GetNext(dir, cell), dir)); break;
                    case '|':
                        if (dir == Direction.Left || dir == Direction.Right)
                        {
                            todo.Push((_grid.GetNext(Direction.Up, cell), Direction.Up));
                            todo.Push((_grid.GetNext(Direction.Down, cell), Direction.Down));
                        }
                        else
                            todo.Push((_grid.GetNext(dir, cell), dir));
                        break;
                    case '-':
                        if (dir == Direction.Up || dir == Direction.Down)
                        {
                            todo.Push((_grid.GetNext(Direction.Left, cell), Direction.Left));
                            todo.Push((_grid.GetNext(Direction.Right, cell), Direction.Right));
                        }
                        else
                            todo.Push((_grid.GetNext(dir, cell), dir));
                        break;
                    case '\\':
                        switch (dir)
                        {
                            case Direction.Right:
                                todo.Push((_grid.GetNext(Direction.Down, cell), Direction.Down)); break;
                            case Direction.Left:
                                todo.Push((_grid.GetNext(Direction.Up, cell), Direction.Up)); break;
                            case Direction.Up:
                                todo.Push((_grid.GetNext(Direction.Left, cell), Direction.Left)); break;
                            case Direction.Down:
                                todo.Push((_grid.GetNext(Direction.Right, cell), Direction.Right)); break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    case '/':
                        switch (dir)
                        {
                            case Direction.Left:
                                todo.Push((_grid.GetNext(Direction.Down, cell), Direction.Down)); break;
                            case Direction.Right:
                                todo.Push((_grid.GetNext(Direction.Up, cell), Direction.Up)); break;
                            case Direction.Down:
                                todo.Push((_grid.GetNext(Direction.Left, cell), Direction.Left)); break;
                            case Direction.Up:
                                todo.Push((_grid.GetNext(Direction.Right, cell), Direction.Right)); break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                }
            }
            var lighten = _lightenGrid.All().Count(c => c.Value);
            return lighten;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var start = (_grid[0, 0], Direction.Right);
            List<(Grid<char>.Cell<char>, Direction)> starts = new List<(Grid<char>.Cell<char>, Direction)>();
            foreach(var cell in _grid.All()) {
                if (cell.X == 0)
                    starts.Add((cell, Direction.Right));
                if (cell.Y == 0)
                    starts.Add((cell, Direction.Down));
                if (cell.X == _grid.XMax - 1)
                    starts.Add((cell, Direction.Left));
                if (cell.Y == _grid.YMax - 1)
                    starts.Add((cell, Direction.Up));
            }

            int max = 0;
            foreach (var s in starts)
            {
                int lighten = CalculateLight(s);
                if (lighten > max) max = lighten;
            }
            return max.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
