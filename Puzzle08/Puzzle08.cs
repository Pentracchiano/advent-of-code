namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

[Description("Haunted Wasteland")]
class Puzzle08 : PuzzleSolution
{
    List<char> moves = new ();
    Dictionary<string, Dictionary<char, string>> map = new ();

    public void Setup(string input)
    {
        var blocks = input.Split(Environment.NewLine + Environment.NewLine);
        moves = blocks[0].ToList();

        var lines = blocks[1].Split(Environment.NewLine);
        map = lines
        .Select(line => {
            var parts = line.Split(" = ");
            var key = parts[0];
            var values = parts[1].Split(", ");
            var mapping = new Dictionary<char, string>()
            {
                ['L'] = values[0][1..],
                ['R'] = values[1][..^1],
            };
            return (parts[0], mapping);
        })
        .ToDictionary();
    }

    public string GetNext(string current, int step) =>
        map[current][moves[step]];

    [Description("How many steps are required to reach ZZZ?")]
    public string SolvePartOne()
    {
        var current = "AAA";
        long steps = 0;
        
        while (current != "ZZZ")
        {
            current = GetNext(current, (int) (steps % moves.Count));
            steps++;
        }
        return steps.ToString();
    }

    [Description("How many steps does it take before you're only on nodes that end with Z?")]
    public string SolvePartTwo() 
    {
        var currents = map.Keys.Where(key => key.EndsWith("A"));
        // find the path size to Z for each.
        // then find the LCM of the paths.
        // i suspect that here the paths are going to be with loops,
        // so we'll need to find the LCM of the loops. or we'll loop for a long time.

        var stepsForEachStart = currents.Select(current => {
            long steps = 0;
            while (!current.EndsWith("Z"))
            {
                current = GetNext(current, (int) (steps % moves.Count));
                steps++;
            }
            return steps;
        });

        var lcm = stepsForEachStart.Aggregate(MathTools.LeastCommonMultiple);
        return lcm.ToString();
    }
}
