namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

[Description("Mirage Maintenance")]
class Puzzle09 : PuzzleSolution
{
    List<List<int>> history = new ();

    public void Setup(string input)
    {
        var lines = input.Split(Environment.NewLine);
        history = lines
        .Select(line => line.Split(" ").Select(int.Parse).ToList())
        .ToList();
    }

    [Description("Analyze your OASIS report and extrapolate the next value for each history. What is the sum of these extrapolated values?")]
    public string SolvePartOne()
    {
        int sum = 0;
        foreach (var entry in history)
        {
            var reconstruction = new List<List<int>>();
            var current = entry;
            while (!current.All(c => c == 0))
            {
                current = current.Pairwise().Select(pair => pair.Item2 - pair.Item1).ToList();
                reconstruction.Add(current);
            }
            // in theory i can skip the last and the second to last.
            // maybe this information (the last is always zero, the second to last is always equal to a constant)
            // can be used to optimize part 2.
            int lastReconstructed = 0;
            foreach (var reconstructed in reconstruction.Reverse<List<int>>().Append(entry))
            {
                lastReconstructed += reconstructed.Last();
            }

            sum += lastReconstructed;
        }
    
        return sum.ToString();
    }

    [Description("")]
    public string SolvePartTwo() 
    {
        throw new NotImplementedException();
    }
}
