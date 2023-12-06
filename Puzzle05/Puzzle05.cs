namespace Advent2023;

using Microsoft.VisualBasic;
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

    [Description("What is the lowest location number that corresponds to any of the initial seed numbers?")]
    public string SolvePartTwo()
    {
        // let's re-parse for this new part. we'll use intervals to create intersections.
        // not that complicated, just unreadable because I used tuples instead of classes... because yeah. why not.
        IEnumerable<(long start, long end)> seedIntervals = seeds.Chunk(2).Select(range => (range[0], range[0] + range[1] - 1));
        IEnumerable<IEnumerable<((long start, long end) destination, (long start, long end) source)>> transformationMapsAsIntervals = transformationMaps.Select(maps => maps.Select(map
            => ((map.destinationStart, map.destinationStart + map.size - 1),
                (map.sourceStart, map.sourceStart + map.size - 1))));

        // idea: compute the intersection with the current map source interval.
        // no intersection, continue to next map in same type.
        // intersection is full: remap the interval, continue to next map type.
        // intersection is partial: split the intervals. remap the intersection which goes into next map type, keep the non-intersected potential "left" and "right" for next maps of same type.

        // at the end, we'll have a list of mapped intervals. just grab the minimum of the start of each interval and we'll have the solution.
        var minValue = long.MaxValue;
        foreach (var seedInterval in seedIntervals)
        {
            // we'll potentially split stuff, so we'll keep track of the new intervals to process with a queue.
            // whatever gets promoted to the next queue will be processed in the next iteration.
            Queue<(long start, long end)> currentSeedIntervals = new([seedInterval]);
            Queue<(long start, long end)> nextSeedIntervals = new();
            foreach (var map in transformationMapsAsIntervals)
            {
                while (currentSeedIntervals.Count > 0)
                {
                    var noIntersectionFound = true;
                    foreach (var transformation in map)
                    {
                        var currentSeedInterval = currentSeedIntervals.Peek();
                        if (currentSeedInterval.start > transformation.source.end || currentSeedInterval.end < transformation.source.start)
                        {
                            // no intersection, continue to next map in same type. if no intersection is found, we'll promote the current interval to the next map type at the end.
                            continue;
                        }

                        // whatever the case now, the current has been processed.
                        currentSeedIntervals.Dequeue();
                        noIntersectionFound = false;

                        if (currentSeedInterval.start >= transformation.source.start && currentSeedInterval.end <= transformation.source.end)
                        {
                            // intersection is full: remap the interval, continue to next map type.
                            nextSeedIntervals.Enqueue((transformation.destination.start + (currentSeedInterval.start - transformation.source.start),
                                                               transformation.destination.start + (currentSeedInterval.end - transformation.source.start)));
                            break;
                        }

                        // intersection is partial: split the intervals., keep the non-intersected potential "left" and "right" for next maps of same type.
                        if (currentSeedInterval.start < transformation.source.start)
                        {
                            currentSeedIntervals.Enqueue((currentSeedInterval.start, transformation.source.start - 1));
                        }
                        if (currentSeedInterval.end > transformation.source.end)
                        {
                            currentSeedIntervals.Enqueue((transformation.source.end + 1, currentSeedInterval.end));
                        }
                        // remap the intersection which goes into next map type
                        nextSeedIntervals.Enqueue((Math.Max(currentSeedInterval.start, transformation.source.start) - transformation.source.start + transformation.destination.start,
                                                         Math.Min(currentSeedInterval.end, transformation.source.end) - transformation.source.start + transformation.destination.start));
                    }

                    if (noIntersectionFound)
                    {
                        // no intersection found, just promote the current interval to the next map type.
                        nextSeedIntervals.Enqueue(currentSeedIntervals.Dequeue());
                    }
                }
                
                // promote the next queue to the current queue.
                currentSeedIntervals = nextSeedIntervals;
                nextSeedIntervals = new();
            }
            
            // at the end, we'll have a list of mapped intervals. just grab the minimum of the start of each interval and we'll have the solution.
            minValue = Math.Min(minValue, currentSeedIntervals.Min(x => x.start));
        }

        return minValue.ToString();
    }
}
