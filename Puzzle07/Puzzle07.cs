namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Description("Camel Cards")]
class Puzzle07 : PuzzleSolution
{
    record Player(string Hand, int Bid);
    List<Player> players = new ();

    Dictionary<char, int> cardPriority = new Dictionary<char, int>()
    {
        ['A'] = 0,
        ['K'] = 1,
        ['Q'] = 2,
        ['J'] = 3,
        ['T'] = 4,
        ['9'] = 5,
        ['8'] = 6,
        ['7'] = 7,
        ['6'] = 8,
        ['5'] = 9,
        ['4'] = 10,
        ['3'] = 11,
        ['2'] = 12,
    };

    public void Setup(string input)
    {
        foreach (var line in Iterators.GetLines(input))
        {
            var split = line.Split();
            players.Add(new(split[0], int.Parse(split[1])));
        }
    }

    private int TypePriority(Player player)
    {
        var hand = player.Hand;

        var matches = hand.GroupBy(c => c);

        // one group == 5 of a kind
        if (matches.Count() == 1)
        {
            return 0;
        }

        // two groups == 4 of a kind or full house
        if (matches.Count() == 2)
        {
            var first = matches.First();
            var second = matches.Last();

            // 4 of a kind
            if (first.Count() == 4 || second.Count() == 4)
            {
                return 1;
            }

            // full house
            return 2;
        }

        // 3 groups == 3 of a kind or two pair
        if (matches.Count() == 3)
        {
            if (matches.Max(c => c.Count()) == 3)
            {
                return 3;
            }

            return 4;
        }

        // 4 groups == 2 of a kind
        if (matches.Count() == 4)
        {
            return 5;
        }

        return 6;
    }

    private int CompareHighestValue(IEnumerable<int> first, IEnumerable<int> second)
    {
        foreach (var (f, s) in first.Zip(second))
        {
            if (f > s)
            {
                return 1;
            }

            if (f < s)
            {
                return -1;
            }
        }
        return 0;
    }

    [Description("Find the rank of every hand in your set. What are the total winnings?")]
    public string SolvePartOne() =>
        players
            .OrderByDescending(TypePriority)
            .ThenByDescending(x => x.Hand.Select(v => cardPriority[v]), Comparer<IEnumerable<int>>.Create(CompareHighestValue))
            .Select((p, i) => p.Bid * (i + 1))
            .Sum()
            .ToString();

    [Description("")]
    public string SolvePartTwo()
    {
        throw new NotImplementedException();
    }
}
