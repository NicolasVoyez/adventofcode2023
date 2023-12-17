using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2023.Solvers
{
    class SolverDay15 : ISolver
    {
        List<string> _instructions = new List<string>();
        List<(string,string)> _splitInstructions = new List<(string, string)>();
        private class Lens
        {
            public Lens(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }
            public int Value { get; set; }
        }
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                _instructions = currentLine.SplitREE(",").ToList();
                _splitInstructions = _instructions.Select(st => {
                    var s = st.SplitREE("=");
                    if (s.Length == 1)
                        return (s[0].Substring(0,s[0].Length - 1), "-");
                    else
                        return (s[0], s[1]);
                  }).ToList();
            }
        }

        public string SolveFirstProblem()
        {
            int sum = 0;
            foreach(var instruction in _instructions)
            {
                int hash = GetHash(instruction);
                sum += hash;
            }

            return sum.ToString();
        }

        private static int GetHash(string instruction)
        {
            var asciified = Encoding.ASCII.GetBytes(instruction);
            var hash = 0;
            foreach (var ascii in asciified)
            {
                hash += ascii;
                hash *= 17;
                hash %= 256;
            }

            return hash;
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            var boxes = new Dictionary<int, List<Lens>>();
            foreach(var (key, action) in _splitInstructions)
            {
                var hash = GetHash(key);
                if (!boxes.TryGetValue(hash, out var focals))
                {
                    focals = new List<Lens>();
                    boxes[hash] = focals;
                }
                if (action == "-")
                {
                    var first = focals.FirstOrDefault(f => f.Name == key);
                    if (first != default)
                        focals.Remove(first);
                }
                else
                {
                    var first = focals.FirstOrDefault(f => f.Name == key);
                    if (first == default)
                        focals.Add(new Lens(key, int.Parse(action)));
                    else
                        first.Value = int.Parse(action);
                }
            }

            var sum = 0;
            foreach (var (id, lenses) in boxes)
            {
                if (lenses.Count == 0) continue;
                for (int i = 0; i < lenses.Count; i++)
                    sum += (id + 1) * (i + 1) * lenses[i].Value;
            }
            return sum.ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
