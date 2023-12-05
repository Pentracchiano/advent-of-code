namespace Advent2023;

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Description("Scratchcards")]
class Puzzle04 : PuzzleSolution
{
    private List<(HashSet<int> winning, HashSet<int> mine)> cards = new ();
    private Regex numberMatcher = new (@"(\d+)");
    private Counter<int> cardAmounts = new Counter<int>();
    public void Setup(string input)
    {
        int i = 1;
        foreach (var line in Iterators.GetLines(input))
        {
            var numbers = line.Split(':')[1];
            var cardValues = numbers.Split('|');

            cards.Add((
                new (numberMatcher.Matches(cardValues[0]).Select(m => int.Parse(m.Value))),
                new (numberMatcher.Matches(cardValues[1]).Select(m => int.Parse(m.Value)))
            ));

            cardAmounts[i]++;
            i++;
        }
    }

    [Description("How many points are they worth in total?")]
    public string SolvePartOne() =>
        cards
        .Select(c => c.winning.Intersect(c.mine))
        .Select(wins => wins.Count())
        .Where(wins => wins > 0)
        .Select(matches => Math.Pow(2, matches - 1))
        .Sum()
        .ToString();
        

    [Description("Including the original set of scratchcards, how many total scratchcards do you end up with?")]
    public string SolvePartTwo()
    {
        int i = 1;
        foreach ((var winning, var mine) in cards)
        {
            var matches = winning.Intersect(mine).Count();
            for (int j = 1; j <= matches; j++)
            {
                cardAmounts[i + j] += cardAmounts[i];
            }

            i++;
        }

        return cardAmounts.Values.Sum(v => (int) v).ToString();
    }
}
