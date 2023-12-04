using AdventOfCode2023.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2023.Solvers
{
    struct Cubes
    {
        public Cubes(int game, int blue, int red, int green)
        {
            Game = game;
            Blue = blue;
            Red = red;
            Green = green;
        }

        public static Cubes GetCubes(int game, string content)
        {
            var split = content.Split(", ");
            int red = 0, blue = 0, green = 0;
            foreach(var c in split)
            {
                var csplit = c.Split(" ");
                int num = int.Parse(csplit[0]);
                switch(csplit[1])
                {
                    case "blue":
                        blue = num; break;
                    case "red":
                        red = num; break;
                    case "green":
                        green = num; break;
                }
            }
            return new Cubes(game, blue, red, green);
        }

        public static Cubes Merge(IList<Cubes> cubes)
        {
            if (cubes.Count == 0)
                throw new NotImplementedException();
            else if (cubes.Count == 1)
                return cubes[0];
            else if (cubes.Count == 2)
                return new Cubes(cubes[0].Game, Math.Max(cubes[0].Blue, cubes[1].Blue), Math.Max(cubes[0].Red, cubes[1].Red), Math.Max(cubes[0].Green, cubes[1].Green));
            else
            {
                Cubes cube = cubes[0];
                for (int i = 1; i < cubes.Count; i++)
                {
                    cube = Cubes.Merge(cube, cubes[i]);
                }
                return cube;
            }
        }

        public static Cubes Merge(params Cubes[] cubes)
        {
            return Cubes.Merge((IList<Cubes>)cubes);
        }

        public bool IsValidForExercice1
        {
            get { return Red <= 12 && Green <= 13 && Blue <= 14; }
        }

        public int Power
        {
            get { return Red * Green * Blue; }
        }

        public int Game { get; }
        public int Blue { get; }
        public int Red { get; }
        public int Green { get; }
    }
    class SolverDay2 : ISolver
    {

        List<List<Cubes>> _input = new List<List<Cubes>>();
        public void InitInput(string content)
        {
            var splitContent = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            foreach (var currentLine in splitContent)
            {
                var split = currentLine.Split(new string[] { "Game ", ": ", "; " }, StringSplitOptions.RemoveEmptyEntries);
                int game = int.Parse(split[0]);
                List<Cubes> cubes= new List<Cubes>();
                for (int i = 1; i < split.Length; i++)
                {
                    cubes.Add(Cubes.GetCubes(game, split[i]));
                }
                _input.Add(cubes);
            }
        }

        public string SolveFirstProblem()
        {
            return _input.Where(l => l.All(c => c.IsValidForExercice1)).Select(l => l[0].Game).Sum().ToString();
        }

        public string SolveSecondProblem(string firstProblemSolution)
        {
            return _input.Select(l => Cubes.Merge(l)).Sum(c => c.Power).ToString();
        }

        public bool Question2CodeIsDone { get; } = true;
        public bool TestOnly { get; } = false;
    }
}
