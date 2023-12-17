using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2023.Solvers
{
    class SolverDay8 : ISolver
    {
        private string Order;
        Dictionary<string, string> Lefts = new Dictionary<string, string>();
        Dictionary<string, string> Rights = new Dictionary<string, string>();
        private const string Start = "AAA";
        private const string End = "ZZZ";
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Order = splitContent[0];

            for (int i = 2; i < splitContent.Length; i++)
            {
                var split = splitContent[i].Split(new string[] { " = (", ", ", ")" }, StringSplitOptions.RemoveEmptyEntries);
                Lefts[split[0]] = split[1];
                Rights[split[0]] = split[2];
            }
        }

        public string SolveFirstProblem()
        {
            BigInteger cnt = 0;
            int pos = 0;
            var current = Start;
            while (current != End)
            {
                current = Order[pos] == 'L' ? Lefts[current] : Rights[current];
                cnt++;
                pos++;
                if (pos == Order.Length)
                    pos = 0;
            }
            return cnt.ToString();

        }

        class LoopInfos
        {
            public BigInteger LoopSize { get; set; }
            public BigInteger StartPos { get; set; }
            public string LoopingKey { get; set; }

            public LoopInfos(string loopingKey, BigInteger startPos, BigInteger loopSize)
            {
                LoopingKey = loopingKey;
                StartPos = startPos;
                LoopSize = loopSize;
            }
        }
        public string SolveSecondProblem(string firstProblemSolution)
        {
            var starts = Lefts.Keys.Where(x => x[2] == 'A').ToList();
            var ends = Lefts.Keys.Where(x => x[2] == 'Z').ToList();

            List<LoopInfos> loops = new List<LoopInfos>();

            foreach (var start in starts)
            {
                BigInteger cnt = 0;
                int pos = 0;
                var current = start;
                var oldPos = new Dictionary<string, Dictionary<int, BigInteger>>();

                while (true)
                {
                    current = Order[pos] == 'L' ? Lefts[current] : Rights[current];
                    cnt++;
                    pos++;
                    if (pos == Order.Length)
                        pos = 0;

                    if (current[2] == 'Z')
                    {
                        if (!oldPos.ContainsKey(current))
                        {
                            oldPos[current] = new Dictionary<int, BigInteger>() { { pos, cnt } };
                        }
                        else
                        {
                            if (oldPos[current].TryGetValue(pos, out var beforeCnt))
                            {
                                loops.Add(new LoopInfos(current, beforeCnt, cnt - beforeCnt));
                                break;
                            }
                            else
                                oldPos[current].Add(pos, cnt);
                        }
                    }
                }

            }

            var ppcm = loops[0].LoopSize;
            for (int i = 1; i < loops.Count; i++)
                ppcm = CalculatePPCM(ppcm, loops[i].LoopSize);

            return ppcm.ToString();
        }

        BigInteger CalculatePPCM(BigInteger a, BigInteger b)
        {
            var p = a * b;
            while (a != b)
                if (a < b)
                    b -= a; 
                else
                    a -= b;
            return p / a;
        }



        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
