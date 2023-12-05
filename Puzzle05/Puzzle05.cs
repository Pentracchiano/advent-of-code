namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Description("If You Give A Seed A Fertilizer")]
class Puzzle05 : PuzzleSolution
{
    private List<long> seeds = [];
    private List<List<(long destinationStart, long sourceStart, long size)>> transformationMaps = [];

    private Regex numberMatcher = new Regex(@"(\d+)");

    public void Setup(string input)
    {
        IEnumerable<string> lines = Iterators.GetLines(input);
        seeds.AddRange(numberMatcher.Matches(lines.First()).Select(x => long.Parse(x.Value)));

        foreach (var line in lines.Skip(1))
        {
            if (!char.IsDigit(line[0]))
            {
                transformationMaps.Add([]);
                continue;
            }

            var numbers = line.Split();
            transformationMaps[^1].Add((long.Parse(numbers[0]), long.Parse(numbers[1]), long.Parse(numbers[2])));
        }
    }

    [Description("What is the lowest location number that corresponds to any of the initial seed numbers?")]
    public string SolvePartOne()
    {
        var minValue = long.MaxValue;
        foreach (var seed in seeds)
        {
            var currentValue = seed;
            foreach (var map in transformationMaps)
            {
                foreach (var transformation in map)
                {
                    if (currentValue >= transformation.sourceStart && currentValue < transformation.sourceStart + transformation.size)
                    {
                        currentValue = transformation.destinationStart + (currentValue - transformation.sourceStart);
                        break;
                    }
                }
            }
            minValue = Math.Min(minValue, currentValue);
        }
        return minValue.ToString();
    }

    [Description("")]
    public string SolvePartTwo()
    {
        throw new NotImplementedException();
    }
}
