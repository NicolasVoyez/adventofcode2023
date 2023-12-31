﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventOfCode2023.Solvers;


var exerciceDone = false;
var exerciceSolved = 0;

while (!exerciceDone)
{
    Console.WriteLine("What problem do you wanna solve ? (default: last)");
    string result = Console.ReadLine();
    Console.WriteLine();
    if (int.TryParse(result, out exerciceSolved))
    {
        exerciceDone = TrySolveExercice(exerciceSolved);
    }

    if (!exerciceDone)
    {
        foreach (var exerciseNumber in Directory.GetFiles("./Inputs/").Select(GetNum)
            .OrderByDescending(x => x))
        {
            exerciceDone = TrySolveExercice(exerciseNumber);
            if (exerciceDone)
            {
                break;
            }
        }
    }
}

Console.WriteLine("Press any key to exit");
Console.ReadKey();


static int GetNum(string path)
{
    var txtNum = path.Split(new[] { "/Inputs/Input" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace(".txt", "");
    return int.Parse(txtNum);
}

static bool TrySolveExercice(int num)
{
    var path = $"./Inputs/Input{num}.txt";
    if (File.Exists(path))
    {
        var input = File.ReadAllText(path);
        ISolver solver = (ISolver)Assembly.GetAssembly(typeof(ISolver))?.GetType($"AdventOfCode2023.Solvers.SolverDay{num}")
            ?.GetConstructor(new Type[0])?.Invoke(new object[0]);
        if (solver != null)
        {

            var testpath = $"./InputsTest/Input{num}.txt";
            var part1Path = $"./InputsTest/Input{num}.part1.txt";
            string testInput = string.Empty;
            bool differentSetForPart2 = false;
            ISolver testSolver = (ISolver)Assembly.GetAssembly(typeof(ISolver))?.GetType($"AdventOfCode2023.Solvers.SolverDay{num}")
                ?.GetConstructor(new Type[0])?.Invoke(new object[0]);
            if (File.Exists(testpath))
            {
                testInput = File.ReadAllText(testpath);
                testSolver.InitInput(testInput);
            }
            else if (File.Exists(part1Path))
            {
                testInput = File.ReadAllText(part1Path);
                testSolver.InitInput(testInput);
                differentSetForPart2 = true;
            }

            if (!solver.TestOnly)
                solver.InitInput(input);

            Console.WriteLine($"Solving exercice of day {num}");
            string testAnswerEx1 = string.Empty;
            if (!string.IsNullOrEmpty(testInput))
            {
                testAnswerEx1 = testSolver.SolveFirstProblem();
                Console.WriteLine("Test Answer to exercice 1 is : " + testAnswerEx1);
            }

            string ex1 = null;
            var sw = Stopwatch.StartNew();
            if (!solver.TestOnly)
            {
                ex1 = solver.SolveFirstProblem();
                Console.WriteLine("Answer to exercice 1 is : " + ex1 + " ... (answer found in " + sw.ElapsedMilliseconds + " ms).");
            }
            if (solver.Question2CodeIsDone)
            {
                if (differentSetForPart2)
                {
                    var part2Path = $"./InputsTest/Input{num}.part2.txt";
                    string test2Input = string.Empty;
                    testSolver = (ISolver)Assembly.GetAssembly(typeof(ISolver))?.GetType($"AdventOfCode2023.Solvers.SolverDay{num}")?.GetConstructor(new Type[0])?.Invoke(new object[0]);
                    testInput = File.ReadAllText(part2Path);
                    testSolver.InitInput(testInput);
                }
                sw = Stopwatch.StartNew();
                if (!string.IsNullOrEmpty(testInput))
                    Console.WriteLine("Test Answer to exercice 2 is : " + testSolver.SolveSecondProblem(testAnswerEx1));
                sw = Stopwatch.StartNew();
                string q2Result = null;
                if (!solver.TestOnly)
                {
                    q2Result = "Answer to exercice 2 is : " + solver.SolveSecondProblem(ex1) + " ... (answer found in " + sw.ElapsedMilliseconds + " ms).";
                }
                if (!string.IsNullOrEmpty(q2Result))
                    Console.WriteLine(q2Result);
            }
            Console.WriteLine();
            return true;
        }

        return false;
    }

    return false;
}


