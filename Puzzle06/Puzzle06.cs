namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Description("Wait For It")]
class Puzzle06 : PuzzleSolution
{
    List<int> times = new();
    List<int> distances = new();
    Regex numberMatcher = new Regex(@"\d+");
    Regex nonNumberMatcher = new Regex(@"[^\d]");
    long longTime = 0;
    long longDistance = 0;

    public void Setup(string input)
    {
        var lines = Iterators.GetLines(input);
        var linesIterator = lines.GetEnumerator();
        linesIterator.MoveNext();
        times = numberMatcher.Matches(linesIterator.Current).Select(i => int.Parse(i.Value)).ToList();
        linesIterator.MoveNext();
        distances = numberMatcher.Matches(linesIterator.Current).Select(i => int.Parse(i.Value)).ToList();

        linesIterator = lines.GetEnumerator();
        linesIterator.MoveNext();
        longTime = long.Parse(nonNumberMatcher.Replace(linesIterator.Current, ""));
        linesIterator.MoveNext();
        longDistance = long.Parse(nonNumberMatcher.Replace(linesIterator.Current, ""));
    }

    private long WaysOfWinning(long time, long distance)
    {
        long ways = 0;
        for (long speed = 1; speed < time; speed++)
        {
            if (distance < speed * (time - speed))
            {
                ways++;
            }
        }
        return ways;
    }

    [Description("Determine the number of ways you could beat the record in each race. What do you get if you multiply these numbers together?")]
    public string SolvePartOne() =>
        (from (int time, int distance) race in times.Zip(distances)
        select WaysOfWinning(race.time, race.distance))
        .Aggregate(1L, (product, current) => product * current)
        .ToString();
        
    [Description("How many ways can you beat the record in this one much longer race?")]
    public string SolvePartTwo() =>
        WaysOfWinning(longTime, longDistance).ToString();
}
