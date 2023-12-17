using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2023.Solvers
{
    class SolverDay12 : ISolver
    {
        List<string> _puzzles = new List<string>();
        List<List<int>> _sizes = new List<List<int>>();
        List<string> _puzzles2 = new List<string>();
        List<List<int>> _sizes2 = new List<List<int>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                var split = currentLine.SplitREE();
                _puzzles.Add(split[0]);
                _puzzles2.Add(split[0] + "?" + split[0] + "?" + split[0] + "?" + split[0] + "?" + split[0]);

                var ints = split[1].SplitAsInt(",").ToList();
                var ex2Ints = new List<int>();
                _sizes.Add(ints);
                for (int i = 0; i < 5; i++)
                {
                    ex2Ints.AddRange(ints);
                }
                _sizes2.Add(ex2Ints);

            }
        }

        public string SolveFirstProblem()
        {
            List<(string, BigInteger)> permutations = new List<(string, BigInteger)>();
            for (int p = 0; p < _puzzles.Count; p++)
            {
                permutations.Add((_puzzles[p], CalculatePermutations(_puzzles[p], _sizes[p])));
            }
            return permutations.Aggregate((BigInteger)0, (sum, kvp) => sum + kvp.Item2).ToString();
        }


        private BigInteger CalculatePermutations(string puzzle, List<int> sizes)
        {
            if (puzzle == "")
                return sizes.Count == 0 ? 1 : 0;

            if (sizes.Count == 0)
                return puzzle.Contains('#') ? 0 : 1;

            var cachekey = puzzle + ";" + string.Join(",", sizes.Select(i => i.ToString()));
            lock (_writeobject)
            {
                if (_cache.TryGetValue(cachekey, out var r))
                    return r.Item1;
            }
            BigInteger result = 0;

            if (puzzle[0] != '#')
                result += CalculatePermutations(puzzle.Substring(1), sizes);
            if (puzzle[0] != '.') {
                if (sizes[0] <= puzzle.Length &&
                    !puzzle.Substring(0, sizes[0]).Contains('.') &&
                    (sizes[0] == puzzle.Length || puzzle[sizes[0]] != '#'))
                    result += CalculatePermutations(sizes[0]  >= puzzle.Length ? "" : puzzle.Substring(sizes[0] + 1), sizes.GetRange(1, sizes.Count - 1));
            }
            lock (_writeobject)
            {
                _cache[cachekey] = (result, 0);
            }
            return result;

        }

        Dictionary<string, (BigInteger, BigInteger)> _cache = new Dictionary<string, (BigInteger, BigInteger)>();
        public string SolveSecondProblem(string firstProblemSolution)
        {
            //InitCache();
            List<(string, BigInteger)> permutations = new List<(string, BigInteger)>();
            BigInteger time = 0;
            Parallel.For(0, _puzzles2.Count,(int p) =>
            {
                var cachekey = _puzzles2[p] + ";" + string.Join(",", _sizes2[p].Select(i => i.ToString()));
                if (_cache.TryGetValue(cachekey, out var res))
                {
                    permutations.Add((_puzzles2[p], res.Item1));
                   // Console.WriteLine($"Permutations computed: {permutations.Count}/{_puzzles2.Count}");
                    time += res.Item2;
                    return;
                }

                Stopwatch sw = Stopwatch.StartNew();
                var result = CalculatePermutations(_puzzles2[p], _sizes2[p]);
                permutations.Add((_puzzles2[p], result));
                lock (_writeobject)
                {
                    File.AppendAllLines(_cachePath, new[] {cachekey + ";" + result + ";" + ((BigInteger)sw.ElapsedMilliseconds).ToString() });
                    time += sw.ElapsedMilliseconds;
                }
                //Console.WriteLine($"Permutations computed: {permutations.Count}/{_puzzles2.Count}");
            });
            Console.WriteLine("Real time in CPU was : {0} ms", time);
            return permutations.Aggregate((BigInteger)0, (sum, kvp) => sum + kvp.Item2).ToString();
        }

        string _cachePath = $"./Caches/Day12.csv";
        private static readonly object _writeobject = new object();

        private void InitCache()
        {
            lock (_writeobject)
            {
                if (!File.Exists(_cachePath))
                {
                    using (File.Create(_cachePath)) { }
                }
                foreach (var line in File.ReadLines(_cachePath))
                {
                    var split = line.SplitREE(";");
                    var cachekey = split[0] + ";" + split[1];
                    _cache[cachekey] = (BigInteger.Parse(split[2]), BigInteger.Parse(split[3]));
                }
            }
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
