namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

[Description("Mirage Maintenance")]
class Puzzle09 : PuzzleSolution
{
    List<List<int>> history = new();

    public void Setup(string input)
    {
        var lines = input.Split(Environment.NewLine);
        history = lines
        .Select(line => line.Split(" ").Select(int.Parse).ToList())
        .ToList();
    }

    private List<List<int>> Reconstruct(List<int> entry)
    {
        var reconstruction = new List<List<int>>();
        var current = entry;
        while (!current.All(c => c == 0))
        {
            current = current.Pairwise().Select(pair => pair.Item2 - pair.Item1).ToList();
            reconstruction.Add(current);
        }
        return reconstruction;
    }

    private string Solve(Func<List<int>, int, int> reconstructSingleValue)
    {
        int sum = 0;
        foreach (var entry in history)
        {
            int lastReconstructed = 0;
            foreach (var reconstructed in Reconstruct(entry).Reverse<List<int>>().Append(entry))
            {
                lastReconstructed = reconstructSingleValue(reconstructed, lastReconstructed);
            }

            sum += lastReconstructed;
        }

        return sum.ToString();
    }

    [Description("Analyze your OASIS report and extrapolate the next value for each history. What is the sum of these extrapolated values?")]
    public string SolvePartOne() =>
        Solve((reconstructed, lastReconstructed) => reconstructed.Last() + lastReconstructed);

    [Description("Analyze your OASIS report again, this time extrapolating the previous value for each history. What is the sum of these extrapolated values?")]
    public string SolvePartTwo() =>
        Solve((reconstructed, lastReconstructed) => reconstructed.First() - lastReconstructed);
}
