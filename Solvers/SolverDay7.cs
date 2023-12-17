using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023.Solvers
{
    class Hand : IComparable<Hand>
    {
        static Dictionary<char, int> points = new Dictionary<char, int>();
        static Dictionary<char, int> points2 = new Dictionary<char, int>();
        public bool CompareOnValue2 = false;
        static Hand()
        {
            int i = 0;
            foreach (var element in new[] { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' })
                points.Add(element, i++);
            foreach (var element in new[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' })
                points2.Add(element, i++);
        }
        public Hand(string cards)
        {
            Cards = cards;
            _value = GetValue();
            _value2 = GetValue2();
        }

        private int GetValue()
        {
            var counts = Cards.GroupBy(c => c).ToList();
            if (counts.Any(g => g.Count() == 5))
                return 7;
            if (counts.Any(g => g.Count() == 4))
                return 6;
            if (counts.Count == 2)
                return 5;
            if (counts.Any(g => g.Count() == 3))
                return 4;
            if (counts.Count == 3)
                return 3;
            if (counts.Count == 4)
                return 2;
            return 1;
        }

        private int GetValue2()
        {
            if (Cards == "JJJJJ")
                return 7;
            var tmp = Cards.Replace("J", "");
            int jokers = 5 - tmp.Length;
            var counts = tmp.GroupBy(c => c).ToList();
            if (counts.Any(g => g.Count() + jokers == 5))
                return 7;
            if (counts.Any(g => g.Count() + jokers == 4))
                return 6;
            if (counts.Count == 2)
                return 5;
            if (counts.Any(g => g.Count() + jokers == 3))
                return 4;
            if (counts.Count == 3)
                return 3;
            if (counts.Count == 4)
                return 2;
            return 1;
        }

        public int CompareTo(Hand? other)
        {
            if (Cards == other.Cards) return 0;
            if (Value != other.Value) return Value > other.Value ? 1 : -1;
            for(int i = 0; i <5; i++)
            {
                var pc1 = CompareOnValue2 ? points2[Cards[i]] : points[Cards[i]];
                var pc2 = CompareOnValue2 ? points2[other.Cards[i]] : points[other.Cards[i]];
                if (pc1 > pc2)
                    return -1;
                else if (pc1 <pc2) return 1;
            }

            throw new NotImplementedException();
        }

        public string Cards { get; }
        private int _value;
        private int _value2;
        public int Value { get { return CompareOnValue2 ? _value2 : _value; } }

        public override string ToString()
        {
            return Cards + " - " + _value.ToString() + " - " + _value2.ToString();
        }
    }
    class SolverDay7 : ISolver
    {
       List<(Hand,int)> _hands = new List<(Hand, int)>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                var split = currentLine.SplitREE();
                _hands.Add((new Hand(split[0]), int.Parse(split[1])));
            }
        }

        public string SolveFirstProblem()
        {
            var orderedHands = _hands.OrderBy(h => h.Item1).ToList();
            BigInteger score = 0;
            for (int i = 0; i < orderedHands.Count; i++)
            {
                var rank = i + 1;
                score += rank * orderedHands[i].Item2;

            }
            return score.ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            foreach (var (card, value) in _hands)
                card.CompareOnValue2 = true;

            var orderedHands = _hands.OrderBy(h => h.Item1).ToList();
            BigInteger score = 0;
            for (int i = 0; i < orderedHands.Count; i++)
            {
                var rank = i + 1;
                score += rank * orderedHands[i].Item2;

            }
            return score.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
