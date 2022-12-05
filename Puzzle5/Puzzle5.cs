using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

[Description("Supply Stacks")]
class Puzzle5 : PuzzleSolution
{
    private List<char>[]? stacks;
    private List<(int amount, int from, int to)> moves = new List<(int amount, int from, int to)>();

    private Regex moveParser = new Regex(@"move (?<amount>\d+) from (?<from>\d+) to (?<to>\d+)");

    private void ReadStack(string stackString)
    {
        string[] stackLines = stackString.Split('\n');

        int stackSize = (stackLines[0].Count() + 1) / 4;

        this.stacks = new List<char>[stackSize];
        for (int i = 0; i < this.stacks.Count(); i++)
        {
            this.stacks[i] = new List<char>();
        }

        foreach (string line in stackLines)
        {
            int i = 0;
            foreach (var chunk in line.Chunk(4))
            {
                var letter = chunk.FirstOrDefault(char.IsAsciiLetterUpper);
                if (letter != default)
                {
                    this.stacks[i].Add(letter);
                }

                i++;
            }
        }

        foreach (var stack in stacks)
        {
            stack.Reverse();
        }
    }

    private void ReadMoves(string movesString)
    {
        foreach (string line in Iterators.GetLines(movesString))
        {
            var match = this.moveParser.Match(line);
            this.moves.Add((
                int.Parse(match.Groups["amount"].Value),
                int.Parse(match.Groups["from"].Value) - 1,
                int.Parse(match.Groups["to"].Value) - 1
            ));
        }
    }

    private string GetTopOfStacks(List<char>[] stacks) =>
        // what happens if there is nothing on top of a stack?
        // currently i ignore them
        string.Join(string.Empty, stacks
            .Where((List<char> stack) => stack.Count() > 0)
            .Select((List<char> stack) => stack.Last())
        );

    private List<char>[] DeepCopyStacks()
    {
        var copy = new List<char>[this.stacks!.Count()];
        for (int i = 0; i < copy.Count(); i++)
        {
            copy[i] = new List<char>(this.stacks![i]);
        }

        return copy;
    }

    private void ApplyMoves(List<char>[] localStacks, bool reverseWhenMoving)
    {
        foreach (var move in this.moves)
        {
            var rangeStart = localStacks[move.from].Count() - move.amount;
            var rangeToAdd = localStacks[move.from].Skip(rangeStart);

            if (reverseWhenMoving)
            {
                rangeToAdd = rangeToAdd.Reverse();
            }

            localStacks[move.to].AddRange(rangeToAdd);
            localStacks[move.from].RemoveRange(rangeStart, move.amount);
        }
    }

    public void Setup(string input)
    {
        string[] problem = input.Split("\n\n");

        ReadStack(problem[0]);
        ReadMoves(problem[1]);
    }


    [Description("After the rearrangement procedure completes, what crate ends up on top of each stack?")]
    public string SolvePartOne()
    {
        List<char>[] localStacks = DeepCopyStacks();
        ApplyMoves(localStacks, true);
        return GetTopOfStacks(localStacks);
    }

    [Description("After the rearrangement procedure completes but without reversing the order at each move, what crate ends up on top of each stack?")]
    public string SolvePartTwo()
    {
        List<char>[] localStacks = DeepCopyStacks();
        ApplyMoves(localStacks, false);
        return GetTopOfStacks(localStacks);
    }
}