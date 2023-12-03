namespace Advent2023;

using System;
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

        // build a graph of nodes, one number can span multiple nodes but will have one id.
        int currentUniqueId = 0;
        foreach (var row in map)
        {
            graph.Add(new List<Node>());
            for (int j = 0; j < row.Length; j++)
            {
                if (!char.IsDigit(row[j]))
                {
                    graph[^1].Add(new Node(null, null, row[j]));
                    continue;
                }

                string numberString = string.Concat(row!.Skip(j).TakeWhile(char.IsDigit));
                int number = int.Parse(numberString);
                j += numberString.Length - 1;
                graph[^1].AddRange(Enumerable.Repeat(new Node(number, currentUniqueId, null), numberString.Length));
                currentUniqueId++;
            }
        }
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

    [Description("What is the sum of all of the gear ratios in your engine schematic?\r\n\r\n")]
    public string SolvePartTwo()
    {
        // go left to right. find a STAR symbol, look for 2 numbers as neighbors.
        // if they are exactly two, then add the multiplication of the numbers to the sum.
        // the numbers are different if they have different ids. use the node.numberId to check.
        // multiply using the node.number, not the node.numberId.
        int sum = 0;

        for (int i = 0; i < graph.Count; i++)
        {
            for (int j = 0; j < graph[i].Count; j++)
            {
                if (graph[i][j].symbol != '*')
                {
                    continue;
                }

                List<int> neighbors = NumberNeighbors(i, j);
                if (neighbors.Count == 2)
                {
                    sum += neighbors[0] * neighbors[1];
                }
            }
        }
        return sum.ToString();
    }

    private List<int> NumberNeighbors(int x, int y)
    {
        HashSet<Node> neighbors = new HashSet<Node>(EqualityComparer<Node>.Create((n1, n2) => n1?.numberId == n2?.numberId, n => n.numberId.GetHashCode()));
        foreach ((int i, int j) in directions)
        {
            if (x + i < graph.Count && x + i >= 0 && y + j < graph[x + i].Count && y + j >= 0)
            {
                if (graph[x + i][y + j].number != null)
                {
                    // in this way we'll have only one node for each number, even if the number spans multiple nodes
                    // this also allows for different numbers that are equal in value
                    neighbors.Add(graph[x + i][y + j]);
                }
            }
        }
        return neighbors.Select(n => n.number!.Value).ToList();
    }
}