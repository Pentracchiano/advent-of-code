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

    [Description("How many steps are required to reach ZZZ?")]
    public string SolvePartOne()
    {
        var current = "AAA";
        long steps = 0;
        
        while (current != "ZZZ")
        {
            var next = map[current][moves[(int) (steps % moves.Count)]];
            current = next;
            steps++;
        }
        return steps.ToString();
    }

        

    [Description("")]
    public string SolvePartTwo() =>
        "";
}
