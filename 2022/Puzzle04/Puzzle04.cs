namespace Advent2022;

using System.ComponentModel;
using System.Text.RegularExpressions;

[Description("Camp Cleanup")]
class Puzzle04 : PuzzleSolution
{
    private List<(Range, Range)> elves = new List<(Range, Range)>();
    private Regex rangeReader = new Regex(@"(?<firstStart>\d+)-(?<firstEnd>\d+),(?<secondStart>\d+)-(?<secondEnd>\d+)");

    private (Range, Range) ReadRanges(string input)
    {
        var match = this.rangeReader.Match(input);
        return (
            new Range(int.Parse(match.Groups["firstStart"].Value), int.Parse(match.Groups["firstEnd"].Value)),
            new Range(int.Parse(match.Groups["secondStart"].Value), int.Parse(match.Groups["secondEnd"].Value))
        );
    }

    private bool RangeFullyContainedCommutative((Range first, Range second) input)
    {
        var union = new Range(new[] { input.first.Start.Value, input.second.Start.Value }.Min(), new[] { input.first.End.Value, input.second.End.Value }.Max());
        return new[] {input.first, input.second}.Contains(union);
    }

    private bool RangesIntersectCommutative((Range first, Range second) input)
    {
        var (first, second) = input;
        return first.Start.Value <= second.End.Value && second.Start.Value <= first.End.Value;
    }

    public void Setup(string input)
    {
        foreach (string line in Iterators.GetLines(input!))
        {
            this.elves.Add(this.ReadRanges(line));
        }
    }

    [Description("In how many assignment pairs does one range fully contain the other?")]
    public string SolvePartOne() =>
        this.elves.Where(this.RangeFullyContainedCommutative).Count().ToString();

    [Description("In how many assignment pairs do the ranges overlap?")]
    public string SolvePartTwo() =>
        this.elves.Where(this.RangesIntersectCommutative).Count().ToString();
}