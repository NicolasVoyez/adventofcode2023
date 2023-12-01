using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2023.Solvers
{
    internal interface ISolver
    {
        public void InitInput(string content);

        public string SolveFirstProblem();


        public string SolveSecondProblem(string firstProblemSolution);
        bool Question2CodeIsDone { get; }
        bool TestOnly { get; }
    }
}
