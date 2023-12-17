using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace AdventOfCode2023.Solvers
{
    class Pipe
    {
        public char Kind { get; set; }
        public bool Virtual { get; set; } = false;
        private int X;
        private int Y;

        public Pipe(char c)
        {
            this.Kind = c;
            Neighbors = new List<Grid<Pipe>.Cell<Pipe>>();
            this.DistanceFromStart = c == 'S' ? 0 : int.MaxValue;
        }
        public List<Grid<Pipe>.Cell<Pipe>> Neighbors { get; set; }
        public int DistanceFromStart { get; set; }
        public override string ToString()
        {
            return Kind.ToString();
        }

        internal void Init(int x, int y, Grid<Pipe> grid)
        {
            X = x;
            Y = y;
            switch (this.Kind)
            {
                case '|':
                    if (y > 0)
                        Neighbors.Add(grid[y - 1, x]);
                    if (y < grid.YMax - 1)
                        Neighbors.Add(grid[y + 1, x]);
                    break;
                case '-':
                    if (x > 0)
                        Neighbors.Add(grid[y, x - 1]);
                    if (x < grid.XMax - 1)
                        Neighbors.Add(grid[y, x + 1]);
                    break;
                case 'L':
                    if (y > 0)
                        Neighbors.Add(grid[y - 1, x]);
                    if (x < grid.XMax - 1)
                        Neighbors.Add(grid[y, x + 1]);
                    break;
                case 'J':
                    if (y > 0)
                        Neighbors.Add(grid[y - 1, x]);
                    if (x > 0)
                        Neighbors.Add(grid[y, x - 1]);
                    break;
                case '7':
                    if (y < grid.YMax - 1)
                        Neighbors.Add(grid[y + 1, x]);
                    if (x > 0)
                        Neighbors.Add(grid[y, x - 1]);
                    break;
                case 'F':
                    if (y < grid.YMax - 1)
                        Neighbors.Add(grid[y + 1, x]);
                    if (x < grid.XMax - 1)
                        Neighbors.Add(grid[y, x + 1]);
                    break;
                case '.':
                case 'S':
                    break;
            }
        }
    }

    class SolverDay10 : ISolver
    {
        private Grid<Pipe> _grid;
        private Grid<Pipe>.Cell<Pipe> _start;
        private string _content;
        public void InitInput(string content)
        {
            _content = content;
            _grid = new Grid<Pipe>(content, (c, x, y) => new Pipe(c));
            _grid.ForEachPerRow(p => p.Value.Init(p.X, p.Y, _grid));
            InitStart();
        }

        private void InitStart()
        {
            _start = _grid.All().First(x => x.Value.DistanceFromStart == 0);
            _start.Value.Neighbors.AddRange(_grid.All().Where(c => c.Value.Neighbors.Contains(_start)));
            if (_start.Value.Neighbors.Contains(_grid[_start.Y + 1, _start.X]))
            {
                if (_start.Value.Neighbors.Contains(_grid[_start.Y, _start.X + 1]))
                {
                    _start.Value.Kind = 'F';
                }
                else if (_start.Value.Neighbors.Contains(_grid[_start.Y, _start.X - 1]))
                {
                    _start.Value.Kind = '7';
                }
                else if (_start.Value.Neighbors.Contains(_grid[_start.Y - 1, _start.X]))
                {
                    _start.Value.Kind = '|';
                }
                else
                    throw new NotImplementedException();
            }
            else if (_start.Value.Neighbors.Contains(_grid[_start.Y - 1, _start.X]))
            {
                if (_start.Value.Neighbors.Contains(_grid[_start.Y, _start.X + 1]))
                {
                    _start.Value.Kind = 'L';
                }
                else if (_start.Value.Neighbors.Contains(_grid[_start.Y, _start.X - 1]))
                {
                    _start.Value.Kind = 'J';
                }
                else
                    throw new NotImplementedException();
            }
            else
            {
                if (_start.Value.Neighbors.Contains(_grid[_start.Y, _start.X + 1]) &&
                    _start.Value.Neighbors.Contains(_grid[_start.Y, _start.X - 1]))
                {
                    _start.Value.Kind = '-';
                }
                else
                    throw new NotImplementedException();
            }
        }

        public string SolveFirstProblem()
        {
            Stack<(Pipe, int)> totreat = new Stack<(Pipe, int)>();
            totreat.Push((_start.Value, 1));
            while (totreat.Count > 0)
            {
                var (curr, dist) = totreat.Pop();
                foreach (var neighbor in curr.Neighbors)
                {
                    if (neighbor.Value.DistanceFromStart > dist)
                    {
                        neighbor.Value.DistanceFromStart = dist;
                        totreat.Push((neighbor.Value, dist + 1));
                    }
                }
            }

            return _grid.All().Where(c => c.Value.DistanceFromStart != int.MaxValue).Max(c => c.Value.DistanceFromStart).ToString();

        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            // different exercice set than 1)
            if (!_grid.All().Any(c => c.Value.DistanceFromStart != int.MaxValue && c.Value.DistanceFromStart != 0))
                SolveFirstProblem();

            _grid = new Grid<Pipe>(_content, (c, x, y) => new Pipe(_grid[y, x].Value.DistanceFromStart == int.MaxValue ? '.' : c));
            var newGrid = new List<IList<Pipe>>();
            for (int y = 0; y < _grid.YMax; y++)
            {
                var line = new List<Pipe>();
                for (int x = 0; x < _grid.XMax - 1; x++)
                {
                    var gridCell = _grid[y, x].Value;
                    var curr = new Pipe(gridCell.Kind);
                    line.Add(curr);
                    if (curr.Kind == '.' || curr.Kind == '|' || curr.Kind == 'J' || curr.Kind == '7')
                        line.Add(new Pipe('.') { Virtual = true });
                    else
                        line.Add(new Pipe('-') { Virtual = true });

                }
                line.Add(_grid[y, _grid.XMax - 1].Value);

                newGrid.Add(line);

                var interLine = new List<Pipe>();
                for (int x = 0; x < line.Count; x++)
                {
                    var kind = line[x].Kind;
                    if (kind == '.' || kind == '-' || kind == 'J' || kind == 'L')
                        interLine.Add(new Pipe('.') { Virtual = true });
                    else
                        interLine.Add(new Pipe('|') { Virtual = true });
                }
                newGrid.Add(interLine);

            }

            _grid = new Grid<Pipe>(newGrid);
            //PrintGrid();

            while (_grid.All().Any(c => c.Value.Kind == '.'))
            {
                var curr = _grid.All().First(c => c.Value.Kind == '.');
                var (group, couldLeave) = ExploreForGroup(curr);
                foreach (var cell in group)
                    cell.Value.Kind = couldLeave ? 'O' : 'I';
            }
            Console.WriteLine();
            Console.WriteLine();
            PrintGrid();
            Console.WriteLine();

            return _grid.Count(c => !c.Value.Virtual && c.Value.Kind == 'I').ToString();

        }

        private void PrintGrid()
        {
            _grid.Print((Grid<Pipe>.Cell<Pipe> x) =>
            {
                if (x.Value.Virtual)
                    return;
                if (x == _start)
                    Console.BackgroundColor = ConsoleColor.Green;

                if (x.Value.Kind == 'I')
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (x.Value.Kind == 'O')
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write(x.Value.Kind);


                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            },
            c => !c.Value.Virtual);
        }

        private (IEnumerable<Grid<Pipe>.Cell<Pipe>>, bool) ExploreForGroup(Grid<Pipe>.Cell<Pipe> start)
        {
            var couldLeave = false;
            HashSet<Grid<Pipe>.Cell<Pipe>> explored = new HashSet<Grid<Pipe>.Cell<Pipe>>();
            Queue<Grid<Pipe>.Cell<Pipe>> toTreat = new Queue<Grid<Pipe>.Cell<Pipe>>();
            toTreat.Enqueue(start);
            while (toTreat.Count > 0)
            {
                var curr = toTreat.Dequeue();
                if (explored.Contains(curr))
                    continue;
                explored.Add(curr);
                foreach (var dir in Directions.All)
                {
                    var next = Next(dir, curr);
                    if (!next.HasValue)
                    {
                        couldLeave = true;
                        continue;
                    }
                    if (explored.Contains(next.Value))
                        continue;
                    switch (next.Value.Value.Kind)
                    {
                        case 'O':
                            couldLeave = true;
                            break;
                        case 'I':
                            couldLeave = false;
                            break;
                        case '.':
                            toTreat.Enqueue(next.Value);
                            break;
                        case '-':
                        case '|':
                        default: // and all the turns
                            break;
                    }

                }
            }
            return (explored, couldLeave);
        }


        //private char[] AlwaysStop = new char[] { 'I', 'O', '.', '|', '-' };
        //private char[] HorizontalMeets = new char[] { 'I', 'O', '.', '|', 'F', 'J', 'L', '7' }; // + FJ or L7
        //private char[] VerticalMeets = new char[] { 'I', 'O', '.', '-', 'F', 'J', 'L', '7' }; // + FJ or L7
        private Grid<Pipe>.Cell<Pipe>? Next(Direction dir, Grid<Pipe>.Cell<Pipe> curr)
        {
            return _grid.FindInDirection(curr, dir, c => true);
            //var meets = dir == Direction.Up || dir == Direction.Down ? VerticalMeets : HorizontalMeets;
            Grid<Pipe>.Cell<Pipe>? next = curr;
            bool foundF = false, foundL = false, foundJ = false, found7 = false;
            while (true)
            {
                next = _grid.FindInDirection(next.Value, dir, n => n.Kind != '.'); //meets.Contains(n.Kind));
                return next;/*
                if (!next.HasValue) return null;
                var kind = next.Value.Value.Kind;
                if (AlwaysStop.Contains(kind))
                    return next;



                switch (dir)
                {
                    case Direction.Right:
                        if (kind == 'F') { foundF = true; foundL = false; }
                        else if (kind == 'L') { foundL = true; foundF = false; }
                        break;
                    case Direction.Left:
                        if (kind == '7') { found7 = true; foundJ = false; }
                        else if (kind == 'J') { foundJ = true; found7 = false; }
                        break;
                    case Direction.Up:
                        if (kind == 'J') { foundJ = true; foundL = false; }
                        else if (kind == 'L') { foundL = true; foundJ = false; }
                        break;
                    case Direction.Down:
                        if (kind == '7') { found7 = true; foundF = false; }
                        else if (kind == 'F') { foundF = true; found7 = false; }
                        break;
                }
                switch (dir)
                {
                    case Direction.Right:
                        if ((foundF && kind == 'J') || (foundL && kind == '7'))
                            return next;
                        break;
                    case Direction.Left:
                        if ((foundJ && kind == 'F') || (found7 && kind == 'L'))
                            return next;
                        break;
                    case Direction.Up:
                        if ((foundJ && kind == 'F') || (foundL && kind == '7'))
                            return next;
                        break;
                    case Direction.Down:
                        if ((foundF && kind == 'J') || (found7 && kind == 'L'))
                            return next;
                        break;
                }
                */

            }
        }


        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
