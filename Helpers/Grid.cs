﻿using AdventOfCode2023.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode2023.Helpers
{
    public class Grid<T>
    {
        public struct Cell<T>
        {
            public int Y { get; }
            public int X { get; }
            public T Value { get; }

            public Cell(int y, int x, T value)
            {
                Y = y;
                X = x;
                Value = value;
            }

            public static bool operator ==(Cell<T> me, Cell<T> other)
            {
                if (ReferenceEquals(me.Value, null) && !ReferenceEquals(other.Value, null))
                    return false;
                if (!ReferenceEquals(me.Value, null) && ReferenceEquals(other.Value, null))
                    return false;
                if (ReferenceEquals(me.Value, null) && ReferenceEquals(other.Value, null))
                    return me.X == other.X && me.Y == other.Y;

                return me.Value.Equals(other.Value) && me.X == other.X && me.Y == other.Y;
            }

            public static bool operator !=(Cell<T> me, Cell<T> other)
            {
                return !(me == other);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Cell<T> cell)) return false;
                return this == cell;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + X.GetHashCode();
                    hash = hash * 23 + Y.GetHashCode();
                    if (!ReferenceEquals(Value, null))
                        hash = hash * 23 + Value.GetHashCode();
                    return hash;
                }
            }

            public override string ToString()
            {
                return $"({X},{Y}) : {Value}";
            }
        }
        // Y,X
        private readonly T[,] _innerGrid;
        public int YMax { get; }
        public int XMax { get; }


        public Grid(string grid, Func<char, T> transform)
        {
            var splitContent = grid.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            YMax = splitContent.Length;
            XMax = splitContent[0].Length;

            _innerGrid = new T[YMax, XMax];

            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    _innerGrid[y, x] = transform(splitContent[y][x]);
                }
            }
        }
        public Grid(string grid, Func<char, int, int, T> transform)
        {
            var splitContent = grid.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            YMax = splitContent.Length;
            XMax = splitContent[0].Length;

            _innerGrid = new T[YMax, XMax];

            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    _innerGrid[y, x] = transform(splitContent[y][x], x, y);
                }
            }
        }
        public Grid(string grid, Func<char, T> transform, T completeWith)
        {
            var splitContent = grid.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            YMax = splitContent.Length;
            XMax = splitContent[0].Length;

            _innerGrid = new T[YMax, XMax];

            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    if (splitContent[y].Length <= x)
                        _innerGrid[y, x] = completeWith;
                    else
                        _innerGrid[y, x] = transform(splitContent[y][x]);
                }
            }
        }


        public Grid(T[,] grid, int yMax, int xMax)
        {
            this.YMax = yMax;
            this.XMax = xMax;
            this._innerGrid = grid;
        }

        public Grid(IList<IList<T>> newGrid)
        {
            YMax = newGrid.Count;
            XMax = newGrid[0].Count;

            _innerGrid = new T[YMax, XMax];

            for (int y = 0; y < YMax; y++)
            {
                var line = newGrid[y];
                for (int x = 0; x < XMax; x++)
                {
                    _innerGrid[y, x] = line[x];
                }
            }
        }

        public IEnumerable<Cell<T>> All()
        {
            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    yield return this[y, x];
                }
            }
        }
        public IEnumerable<Cell<T>> GetRow(int y)
        {
            if (y >= 0 && y < YMax)
            {
                for (int x = 0; x < XMax; x++)
                {
                    yield return this[y, x];
                }
            }
        }
        public IEnumerable<T> GetRowValues(int y)
        {
            foreach (var cell in GetRow(y))
                yield return cell.Value;
        }

        public IEnumerable<Cell<T>> GetColumn(int x)
        {
            if (x >= 0 && x < XMax)
            {
                for (int y = 0; y < YMax; y++)
                {
                    yield return this[y, x];
                }
            }
        }
        public IEnumerable<T> GetColumnValues(int x)
        {
            foreach (var cell in GetColumn(x))
                yield return cell.Value;
        }



        internal int Count(Func<Cell<T>, bool> countPredicate) => All().Count(countPredicate);
        internal int Count(Func<T, bool> countPredicate) => All().Count(c => countPredicate(c.Value));

        public Cell<T>? GetNext(Direction d, Cell<T> cell, bool wrap = false, Func<int, int, Cell<T>> defaultCtor = null)
        {
            switch (d)
            {
                case Direction.Right:
                    var newX = cell.X + 1;
                    if (newX > XMax - 1)
                    {
                        if (!wrap)
                            return defaultCtor == null ? null : defaultCtor(newX, cell.Y);
                        newX = 0;
                    }
                    return this[cell.Y, newX];
                case Direction.Left:
                    newX = cell.X - 1;
                    if (newX < 0)
                    {
                        if (!wrap)
                            return defaultCtor == null ? null : defaultCtor(newX, cell.Y);
                        newX = XMax - 1;
                    }
                    return this[cell.Y, newX];
                case Direction.Up:
                    var newY = cell.Y - 1;
                    if (newY < 0)
                    {
                        if (!wrap)
                            return defaultCtor == null ? null : defaultCtor(cell.X, newY);
                        newY = YMax - 1;
                    }
                    return this[newY, cell.X];
                case Direction.Down:
                    newY = cell.Y + 1;
                    if (newY > YMax - 1)
                    {
                        if (!wrap)
                            return defaultCtor == null ? null : defaultCtor(cell.X, newY);
                        newY = 0;
                    }
                    return this[newY, cell.X];
            }
            throw new Exception("Should never happen");
        }

        public IEnumerable<Cell<T>> Around(Cell<T> cell, bool withDiagonals = true)
        {
            return Around(cell.Y, cell.X, withDiagonals);
        }
        public IEnumerable<Cell<T>> Around(int y, int x, bool withDiagonals = true)
        {
            if (y > 0)
            {
                if (withDiagonals && x > 0)
                    yield return this[y - 1, x - 1];
                yield return this[y - 1, x];
                if (withDiagonals && x < XMax - 1)
                    yield return this[y - 1, x + 1];
            }

            if (x > 0)
                yield return this[y, x - 1];

            if (x < XMax - 1)
                yield return this[y, x + 1];

            if (y < YMax - 1)
            {
                if (withDiagonals && x > 0)
                    yield return this[y + 1, x - 1];
                yield return this[y + 1, x];
                if (withDiagonals && x < XMax - 1)
                    yield return this[y + 1, x + 1];
            }
        }

        public IEnumerable<Cell<T>> GetFirstInEachDirection(int y, int x, Predicate<T> condition, Func<int, int, Cell<T>> defaultCtor = null, bool noDiagonal = false)
        {
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    if (noDiagonal && dx != 0 && dy != 0)
                        continue;
                    var cell = FindInDirection(y, x, dy, dx, condition, defaultCtor);
                    if (cell.HasValue)
                        yield return cell.Value;
                }
        }

        public Cell<T>? FindInDirection(Cell<T> cell, Direction dir, Predicate<T> condition, Func<int, int, Cell<T>> defaultCtor = null, bool wrap = false)
        {
            var dx = dir == Direction.Right ? 1 : (dir == Direction.Left ? -1 : 0);
            var dy = dir == Direction.Up ? -1 : (dir == Direction.Down ? 1 : 0);
            return FindInDirection(cell.Y, cell.X, dy, dx, condition, defaultCtor, wrap);
        }

        private Cell<T>? FindInDirection(int y, int x, int dy, int dx, Predicate<T> condition, Func<int, int, Cell<T>> defaultCtor, bool wrap = false)
        {
            int startY = y;
            int startX = x;
            while (true)
            {
                y += dy;
                x += dx;

                if (!wrap && (y < 0 || x < 0 || y > YMax - 1 || x > XMax - 1))
                    return defaultCtor == null ? null : defaultCtor(x, y);
                if (wrap)
                {
                    if (y < 0) y = YMax - 1;
                    if (x < 0) x = XMax - 1;
                    if (y > YMax - 1) y = 0;
                    if (x > XMax - 1) x = 0;

                    if (y == startY && x == startX)
                        return defaultCtor == null ? null : defaultCtor(x, y);
                }

                if (condition(_innerGrid[y, x]))
                    return this[y, x];
            }
        }

        public void ForEachPerRow(Action<Cell<T>> action, bool reverseOrder = false)
        {
            for (int y = 0; y < YMax; y++)
            {
                if (reverseOrder)
                    for (int x = XMax - 1; x >= 0; x--)
                        action(this[y, x]);
                else
                    for (int x = 0; x < XMax; x++)
                        action(this[y, x]);
            }
        }
        public void ForEachPerColumn(Action<Cell<T>> action, bool reverseOrder = false)
        {
            for (int x = 0; x < XMax; x++)
            {
                if (reverseOrder)
                    for (int y = YMax - 1; y >= 0; y--)
                        action(this[y, x]);
                else
                    for (int y = 0; y < YMax; y++)
                        action(this[y, x]);
            }
        }

        public Cell<T> this[int y, int x]
        {
            get { return new Cell<T>(y, x, _innerGrid[y, x]); }
            set { Set(y, x, value.Value); }
        }

        public void Set(int y, int x, T value) => _innerGrid[y, x] = value;

        public void Print(Action<T> printElement)
        {
            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    printElement(_innerGrid[y, x]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        public void Print(Action<Cell<T>> printElement)
        {
            for (int y = 0; y < YMax; y++)
            {
                for (int x = 0; x < XMax; x++)
                {
                    printElement(this[y, x]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        public void Print(Action<Cell<T>> printElement, Predicate<Cell<T>> anyInLineToKeepReturn)
        {
            for (int y = 0; y < YMax; y++)
            {
                bool skipLine = false;
                for (int x = 0; x < XMax; x++)
                {
                    printElement(this[y, x]);
                    if (!skipLine)
                        skipLine = anyInLineToKeepReturn(this[y, x]);
                }
                if (!skipLine)
                    Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
