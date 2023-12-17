using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace AdventOfCode2023.Solvers
{
    struct Range
    {
        public Range(BigInteger start, BigInteger end)
        {
            Start = start;
            End = end;
        }

        public BigInteger Start { get; }
        public BigInteger End { get; }

        public override string ToString()
        {
            return Start.ToString() + " - " + End.ToString();
        }

        /// <summary>
        /// First is not splittable anymore
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public TransformResult Transform(TransformRange next)
        {
            /*      ++++++++ 
             *                   +++++++       */
            List<TransformRange> finalRanges = new List<TransformRange>();
            if (End <= next.FromStart || Start > next.FromEnd)
                return null;

            if (Start <= next.FromStart)
            {
                var distStart = next.FromStart - Start;
                var first = new Range(Start, next.FromStart);
                List<Range> notTreated = new List<Range>();
                if (first.Start != first.End)
                    notTreated.Add(first);
                if (End < next.FromEnd)
                {
                    /*      ++++++++ 
                     *          +++++++       */
                    var mergeLen = End - next.FromStart;
                    var merged = new Range(next.ToStart, next.ToStart + mergeLen);
                    return new TransformResult(notTreated, merged.Start == merged.End ? null : merged) ;
                }
                else
                {
                    /*    ++++++++++++
                     *       +             */
                    var mergeLen = next.ToEnd - next.ToStart;
                    var merged = new Range(next.ToStart, next.ToStart + mergeLen);
                    var end = new Range(Start + distStart + mergeLen, End);
                    if (end.Start != end.End)
                        notTreated.Add(end);
                    return new TransformResult(notTreated, merged.Start == merged.End ? null : merged);
                }
            }
            else
            {
                if (next.FromEnd < End)
                {
                    /*      ++++++++ 
                     *    ++++++       */
                    var distStart = Start - next.FromStart;
                    var merged = new Range(next.ToStart + distStart, next.ToEnd);
                    List<Range> notTreated = new List<Range>();
                    var end = new Range(next.FromEnd, End);
                    if (end.Start != end.End)
                        notTreated.Add(end);
                    return new TransformResult(notTreated, merged.Start == merged.End ? null : merged);
                }
                else
                {
                    /*      ++++++++ 
                     *   +++++++++++++++     */
                    var distStart = Start - next.FromStart;
                    var merged = new Range(next.ToStart + distStart, next.ToStart + distStart + (End - Start));
                    return new TransformResult(new List<Range>(), merged.Start == merged.End ? null : merged);
                }

            }
        }
    }

    class TransformResult
    {
        public TransformResult(IList<Range> notTreated, Range? result)
        {
            NotTreated = notTreated;
            Result = result;
        }

        public IList<Range> NotTreated { get; }
        public Range? Result { get; }
    }

    struct TransformRange
    {
        public TransformRange(IList<BigInteger> input) : this(input[0], input[1], input[2])
        {
        }
        public TransformRange(BigInteger toStart, BigInteger fromStart, BigInteger rangeSize) : this(fromStart, fromStart + rangeSize, toStart, toStart + rangeSize)
        {
        }

        public TransformRange(BigInteger fromStart, BigInteger fromEnd, BigInteger toStart, BigInteger toEnd)
        {
            FromStart = fromStart;
            FromEnd = fromEnd;
            ToStart = toStart;
            ToEnd = toEnd;
        }

        public BigInteger FromStart { get; }
        public BigInteger FromEnd { get; }
        public BigInteger ToStart { get; }
        public BigInteger ToEnd { get; }

    }
    class SolverDay5 : ISolver
    {
        List<BigInteger> _seeds = new List<BigInteger>();
        List<List<TransformRange>> _transformers = new List<List<TransformRange>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            _seeds = splitContent[0].Replace("seeds: ", "").SplitAsBigInteger().ToList();

            var currentTransform = new List<TransformRange>();
            for (int i = 2; i < splitContent.Length; i++)
            {
                var currentLine = splitContent[i];
                if (string.IsNullOrEmpty(currentLine) || !char.IsDigit(currentLine[0]))
                {
                    if (currentTransform.Count > 0)
                    {
                        _transformers.Add(currentTransform);
                        currentTransform = new List<TransformRange>();
                    }
                    continue;
                }
                currentTransform.Add(new TransformRange(currentLine.SplitAsBigInteger()));
            }

            if (currentTransform.Count > 0)
                _transformers.Add(currentTransform);
        }

        public string SolveFirstProblem()
        {
            List<BigInteger> transformed = new List<BigInteger>(_seeds);
            TransformSeeds(transformed);
            return transformed.Min().ToString();
        }

        private void TransformSeeds(List<BigInteger> transformed)
        {
            Parallel.For(0, transformed.Count, i =>
            {
                var curr = transformed[i];
                foreach (var transformerList in _transformers)
                {
                    foreach (var transformer in transformerList)
                    {
                        if (curr >= transformer.FromStart && curr < transformer.FromEnd)
                        {
                            curr = transformer.ToStart + (curr - transformer.FromStart);
                            break;
                        }
                    }
                    transformed[i] = curr;
                }
            });
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            List<Range> transformedRanges = new List<Range>();
            for (int i = 0; i * 2 <= _seeds.Count; i += 2)
            {
                transformedRanges.Add(new Range(_seeds[i], _seeds[i] + _seeds[i +1]));
            }
            List<Range> endRanges = new List<Range>();

            foreach (var initialRange in transformedRanges)
            {
                List<Range> newRanges = new List<Range>();
                newRanges.Add(initialRange);
                foreach (var transformerList in _transformers)
                {
                    Stack<Range> toTransform = new Stack<Range>();
                    foreach (var r in newRanges)
                        toTransform.Push(r);
                    newRanges = new List<Range>();
                    List<TransformRange> remainingTransformers = new List<TransformRange>(transformerList);
                    while (toTransform.Count > 0)
                    {
                        var r = toTransform.Pop();
                        bool found = false;
                        foreach (var transformer in remainingTransformers.ToList())
                        {
                            //remainingTransformers.Remove(transformer);
                            var res = r.Transform(transformer);
                            if (res != null && res.Result.HasValue)
                            {
                                found = true;
                                newRanges.Add(res.Result.Value);
                                foreach (var untreated in res.NotTreated)
                                    toTransform.Push(untreated);
                                break;
                            }
                        }
                        if (!found)
                        {
                            newRanges.Add(r);
                        }
                    }
                }
                endRanges.AddRange(newRanges);
            }

            return endRanges.Min(r => r.Start).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
