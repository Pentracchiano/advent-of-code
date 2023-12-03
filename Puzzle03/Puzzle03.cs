namespace Advent2023;

using System.ComponentModel;

[Description("Gear Ratios")]
class Puzzle03 : PuzzleSolution
{
    record Node
    (
        int? number,
        int? numberId, // to check if two nodes are the same number, not if they have same value
        char? symbol
    );

    private List<List<Node>> graph = new ();
    private List<string> map = new ();
    private (int, int)[] directions = [(0, 1), (0, -1), (1, 0), (-1, 0), (1, 1), (-1, -1), (1, -1), (-1, 1)];
    public void Setup(string input)
    {
        map = Iterators.GetLines(input).ToList();
    }

    private bool IsAdjacentToSymbol(Queue<(int, int)> number)
    {
        while(number.Count > 0)
        {
            (int x, int y) = number.Dequeue();
            foreach ((int i, int j) in directions)
            {
                if (x + i < map.Count && x + i >= 0 && y + j < map[x + i].Length && y + j >= 0)
                {
                    if (!char.IsDigit(map[x + i][y + j]) && map[x + i][y + j] != '.')
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    [Description("What is the sum of all of the part numbers in the engine schematic?")]
    public string SolvePartOne()
    {
        // go left to right. find a number, put digits in a queue,
        // then run a 1 turn BFS to find whether the number is adjacent to 
        // a symbol which is not a number or a dot. if so, add to sum.
        int sum = 0;
        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                if (char.IsDigit(map[i][j]))
                {
                    var number = new Queue<(int, int)>(
                        // really readable thing which means take al subsequent digits coordinates. why not a for, I am asking myself? because!
                        map[i].Skip(j).Select((c, k) => (c, k)).TakeWhile(v => char.IsDigit(v.c)).Select(v => (i, j + v.k))
                    );
                    int numberSize = number.Count;

                    if (IsAdjacentToSymbol(number))
                    {
                        sum += int.Parse(map[i].AsSpan(j, numberSize));
                    }

                    j += numberSize - 1;
                }
            }
        }
        return sum.ToString();
    }

    [Description("")]
    public string SolvePartTwo()
    {
        return "0";
    }
}