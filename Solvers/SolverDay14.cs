using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023.Solvers
{
    class SolverDay14 : ISolver
    {
        private HashSet<Point> _rocks = new HashSet<Point>();
        private List<Point> _balls = new List<Point>();
        Grid<char> _grid;
        private Dictionary<int, HashSet<int>> RocksAndBordersPerX = new Dictionary<int, HashSet<int>>();
        private Dictionary<int, HashSet<int>> RocksAndBordersPerY = new Dictionary<int, HashSet<int>>();
        public void InitInput(string content)
        {
            _grid = new Grid<char>(content, c => c);
            foreach (var pt in _grid.All())
            {
                if (pt.Value == 'O')
                    _balls.Add(new Point(pt.Y, pt.X));
                else if (pt.Value == '#')
                    _rocks.Add(new Point(pt.Y, pt.X));
            }

            foreach (var rock in _rocks)
            {
                if (!RocksAndBordersPerX.TryGetValue(rock.X, out var xpts))
                {
                    xpts = new HashSet<int>();
                    RocksAndBordersPerX[rock.X] = xpts;
                }
                xpts.Add(rock.Y);
                if (!RocksAndBordersPerY.TryGetValue(rock.Y, out var ypts))
                {
                    ypts = new HashSet<int>();
                    RocksAndBordersPerY[rock.Y] = ypts;
                }
                ypts.Add(rock.X);
            }
            RocksAndBordersPerY[-1] = new HashSet<int>();
            RocksAndBordersPerY[_grid.YMax] = new HashSet<int>();
            for (int x = 0; x < _grid.XMax; x++)
            {
                RocksAndBordersPerY[-1].Add(x);
                RocksAndBordersPerY[_grid.YMax].Add(x);
                if (!RocksAndBordersPerX.TryGetValue(x, out var xpts))
                {
                    xpts = new HashSet<int>();
                    RocksAndBordersPerX[x] = xpts;
                }
                xpts.Add(-1);
                xpts.Add(_grid.YMax);
            }
            RocksAndBordersPerX[-1] = new HashSet<int>();
            RocksAndBordersPerX[_grid.YMax] = new HashSet<int>();
            for (int y = 0; y < _grid.YMax; y++)
            {
                RocksAndBordersPerX[-1].Add(y);
                RocksAndBordersPerX[_grid.YMax].Add(y);
                if (!RocksAndBordersPerY.TryGetValue(y, out var ypts))
                {
                    ypts = new HashSet<int>();
                    RocksAndBordersPerY[y] = ypts;
                }
                ypts.Add(-1);
                ypts.Add(_grid.XMax);
            }
        }

        public string SolveFirstProblem()
        {
            var balls = PushUp(_balls);
            //Print(balls);
            return balls.Sum(p => _grid.XMax - p.Y).ToString();
        }

        private void Print(HashSet<Point> balls)
        {
            Print(balls, new Point(-10, -10));
        }
        private void Print(HashSet<Point> balls, Point emphasis)
        {
            Console.WriteLine();
            for (int y = 0; y < _grid.YMax; y++)
            {
                for (int x = 0; x < _grid.XMax; x++)
                {
                    var pt = new Point(y, x);
                    if (emphasis == pt)
                        Console.ForegroundColor = ConsoleColor.Red;
                    if (balls.Contains(pt))
                        Console.Write('O');
                    else if (_rocks.Contains(pt))
                        Console.Write('#');
                    else
                        Console.Write('.');
                    if (emphasis == pt)
                        Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine(" " + (_grid.YMax - y));
            }
            Console.WriteLine();
        }
        #region Ugly Copy
        private HashSet<Point> PushUp(IEnumerable<Point> balls)
        {
            var result = new Dictionary<int, HashSet<int>>();
            foreach (var (x, hs) in RocksAndBordersPerX)
                result[x] = new HashSet<int>(hs);
            HashSet<Point> newBalls = new HashSet<Point>();

            foreach (var rolling in balls)
            {
                var stop = result[rolling.X].Where(y => y < rolling.Y).Max();
                for (int y = stop + 1; y <= _grid.YMax; y++)
                {
                    var pt = new Point(y, rolling.X);
                    if (!result[pt.X].Contains(pt.Y))
                    {
                        result[pt.X].Add(pt.Y);
                        newBalls.Add(pt);
                        break;
                    }
                }
            }

            return newBalls;
        }

        private HashSet<Point> PushDown(IEnumerable<Point> balls)
        {
            var result = new Dictionary<int, HashSet<int>>();
            foreach (var (x, hs) in RocksAndBordersPerX)
                result[x] = new HashSet<int>(hs);
            HashSet<Point> newBalls = new HashSet<Point>();

            foreach (var rolling in balls)
            {
                var stop = result[rolling.X].Where(y => y > rolling.Y).Min();
                for (int y = stop - 1; y >= 0; y--)
                {
                    var pt = new Point(y, rolling.X);
                    if (!result[pt.X].Contains(pt.Y))
                    {
                        result[pt.X].Add(pt.Y);
                        newBalls.Add(pt);
                        break;
                    }
                }
            }

            return newBalls;
        }
        private HashSet<Point> PushRight(IEnumerable<Point> balls)
        {
            var result = new Dictionary<int, HashSet<int>>();
            foreach (var (y, hs) in RocksAndBordersPerY)
                result[y] = new HashSet<int>(hs);
            HashSet<Point> newBalls = new HashSet<Point>();

            foreach (var rolling in balls)
            {
                var stop = result[rolling.Y].Where(x => x > rolling.X).Min();
                for (int x = stop - 1; x >= 0; x--)
                {
                    var pt = new Point(rolling.Y, x);
                    if (!result[pt.Y].Contains(pt.X))
                    {
                        result[pt.Y].Add(pt.X);
                        newBalls.Add(pt);
                        break;
                    }
                }
            }

            return newBalls;
        }
        private HashSet<Point> PushLeft(IEnumerable<Point> balls)
        {
            var result = new Dictionary<int, HashSet<int>>();
            foreach (var (y, hs) in RocksAndBordersPerY)
                result[y] = new HashSet<int>(hs);
            HashSet<Point> newBalls = new HashSet<Point>();

            foreach (var rolling in balls)
            {
                var stop = result[rolling.Y].Where(x => x < rolling.X).Max();
                for (int x = stop + 1; x <= _grid.XMax; x++)
                {
                    var pt = new Point(rolling.Y, x);
                    if (!result[pt.Y].Contains(pt.X))
                    {
                        result[pt.Y].Add(pt.X);
                        newBalls.Add(pt);
                        break;
                    }
                }
            }

            return newBalls;
        }
        #endregion

        // first apparition of the configuration
        Dictionary<string, BigInteger> _cache = new Dictionary<string, BigInteger>();
        public string SolveSecondProblem(string firstProblemSolution)
        {
            BigInteger ROUNDS = 1000000000l * 4;
            var balls = new HashSet<Point>(_balls);
            for (BigInteger i = 0; i < ROUNDS; i++)
            {
                var ballsKey = balls.OrderBy(p => p.X*1000 + p.Y).Aggregate("", (x, p) => x + "," + p.ToString());
                if (_cache.TryGetValue(ballsKey, out BigInteger prevIdx))
                {
                    var cycle = i - prevIdx;
                    var runs = (ROUNDS - i) / cycle;
                    i += runs * cycle;
                }
                else
                    _cache.Add(ballsKey, i);
                switch ((int)(i % 4))
                {
                    case 0:
                        balls = PushUp(balls); break;
                    case 1:
                        balls = PushLeft(balls); break;
                    case 2:
                        balls = PushDown(balls); break;
                    case 3:
                        balls = PushRight(balls); break;
                }
            }
            return balls.Sum(p => _grid.XMax - p.Y).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
