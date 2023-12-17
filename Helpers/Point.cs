using System.Numerics;

namespace AdventOfCode2023.Helpers
{
    public struct Point
    {
        public int Y { get; }
        public int X { get; }

        public Point(int y, int x)
        {
            Y = y;
            X = x;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point pt))
                return false;
            return Equals(pt);
        }
        public bool Equals(Point obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return X == obj.X && Y == obj.Y;
        }
        public static bool operator == (Point pt1, Point pt2)
        {
            return pt1.Equals(pt2);
        }
        public static bool operator !=(Point pt1, Point pt2)
        {
            return !(pt1 == pt2);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ 27 * Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(X:" + X + " Y:" + Y + ")";
        }

        // NOTE: D24 put up - and Down + , while it's inverted for days 9 & 17
        public Point ToDirection(Direction d, int qty = 1)
        {
            return d switch
            {
                Direction.Right => new Point(Y, X + qty),
                Direction.Left => new Point(Y, X - qty),
                Direction.Up => new Point(Y - qty, X),
                Direction.Down => new Point(Y + qty, X),
                _ => throw new ArgumentException("not a direction"),
            };
        }

        internal int ManhattanDistance(Point other)
        {
            return Math.Abs(Y - other.Y) + Math.Abs(X - other.X);
        }

        internal IEnumerable<Point> Around()
        {
            yield return new Point(Y, X + 1);
            yield return new Point(Y, X - 1);
            yield return new Point(Y + 1, X);
            yield return new Point(Y - 1, X);
        }
        internal IEnumerable<Point> Around(int minX, int minY, int maxX, int maxY)
        {
            if (X + 1 <= maxX && Y >= minY && Y <= maxY)
                yield return new Point(Y, X + 1);
            if (X - 1 >= minX && Y >= minY && Y <= maxY)
                yield return new Point(Y, X - 1);
            if (Y + 1 <= maxY && X >= minX && X <= maxX)
                yield return new Point(Y + 1, X);
            if (Y - 1 >= minY && X >= minX && X <= maxX)
                yield return new Point(Y - 1, X);
        }
    }

    public struct BigPoint
    {
        public BigInteger Y { get; }
        public BigInteger X { get; }

        public BigPoint(BigInteger y, BigInteger x)
        {
            Y = y;
            X = x;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BigPoint pt))
                return false;
            return Equals(pt);
        }
        public bool Equals(BigPoint obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return X == obj.X && Y == obj.Y;
        }
        public static bool operator ==(BigPoint pt1, BigPoint pt2)
        {
            return pt1.Equals(pt2);
        }
        public static bool operator !=(BigPoint pt1, BigPoint pt2)
        {
            return !(pt1 == pt2);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ 27 * Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(X:" + X + " Y:" + Y + ")";
        }

        // NOTE: D24 put up - and Down + , while it's inverted for days 9 & 17
        public BigPoint ToDirection(Direction d, int qty = 1)
        {
            return d switch
            {
                Direction.Right => new BigPoint(Y, X + qty),
                Direction.Left => new BigPoint(Y, X - qty),
                Direction.Up => new BigPoint(Y - qty, X),
                Direction.Down => new BigPoint(Y + qty, X),
                _ => throw new ArgumentException("not a direction"),
            };
        }

        internal BigInteger ManhattanDistance(BigPoint other)
        {
            return BigInteger.Abs(Y - other.Y) + BigInteger.Abs(X - other.X);
        }

        internal IEnumerable<BigPoint> Around()
        {
            yield return new BigPoint(Y, X + 1);
            yield return new BigPoint(Y, X - 1);
            yield return new BigPoint(Y + 1, X);
            yield return new BigPoint(Y - 1, X);
        }
        internal IEnumerable<BigPoint> Around(int minX, int minY, int maxX, int maxY)
        {
            if (X + 1 <= maxX && Y >= minY && Y <= maxY)
                yield return new BigPoint(Y, X + 1);
            if (X - 1 >= minX && Y >= minY && Y <= maxY)
                yield return new BigPoint(Y, X - 1);
            if (Y + 1 <= maxY && X >= minX && X <= maxX)
                yield return new BigPoint(Y + 1, X);
            if (Y - 1 >= minY && X >= minX && X <= maxX)
                yield return new BigPoint(Y - 1, X);
        }
    }
}
