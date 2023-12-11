namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;

[Description("Pipe Maze")]
class Puzzle10 : PuzzleSolution
{
    private record Node(int row, int column);
    // what i built here is a directed graph.
    // in order for 2 pipes to be connected, they need to be neighbors on both sides.
    private Dictionary<Node, List<Node>> map = new();
    private Node animal = new(0, 0);
    int maxColumn = 0;
    int maxRow = 0;

    private void SetPipeNeighbors(Node node, char pipe, int maxRow, int maxColumn)
    {
        var (i, j) = node;
        var neighbors = map.GetValueOrSetDefault(node, new List<Node>());

        switch (pipe)
        {
            case 'J':
                if (i - 1 >= 0)
                {
                    neighbors.Add(new Node(i - 1, j));
                }
                if (j - 1 >= 0)
                {
                    neighbors.Add(new Node(i, j - 1));
                }
                break;
            case 'L':
                if (i - 1 >= 0)
                {
                    neighbors.Add(new Node(i - 1, j));
                }
                if (j + 1 < maxColumn)
                {
                    neighbors.Add(new Node(i, j + 1));
                }
                break;
            case '7':
                if (i + 1 < maxRow)
                {
                    neighbors.Add(new Node(i + 1, j));
                }
                if (j - 1 >= 0)
                {
                    neighbors.Add(new Node(i, j - 1));
                }
                break;
            case 'F':
                if (i + 1 < maxRow)
                {
                    neighbors.Add(new Node(i + 1, j));
                }
                if (j + 1 < maxColumn)
                {
                    neighbors.Add(new Node(i, j + 1));
                }
                break;
            case '|':
                if (i - 1 >= 0)
                {
                    neighbors.Add(new Node(i - 1, j));
                }
                if (i + 1 < maxRow)
                {
                    neighbors.Add(new Node(i + 1, j));
                }
                break;
            case '-':
                if (j - 1 >= 0)
                {
                    neighbors.Add(new Node(i, j - 1));
                }
                if (j + 1 < maxColumn)
                {
                    neighbors.Add(new Node(i, j + 1));
                }
                break;
            default:
                throw new Exception("Unknown pipe");
        }
    }

    public void Setup(string input)
    {
        var lines = input.Split(Environment.NewLine);
        (maxColumn, maxRow) = (lines[0].Length, lines.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == '.')
                {
                    continue;
                }
                var node = new Node(i, j);
                if (lines[i][j] == 'S')
                {
                    animal = node;
                    continue;
                }
                SetPipeNeighbors(node, lines[i][j], maxRow, maxColumn);
            }
        }
    }

    private IEnumerable<Node> GetConnectedNeighbors(Node node)
    {
        var neighbors = map.GetValueOrDefault(node, new List<Node>());
        return neighbors.Where(neighbor => map.GetValueOrDefault(neighbor, new List<Node>()).Contains(node));
    }

    private bool IsAnimalLoop(Dictionary<Node, List<Node>> map, out int loopSize)
    {
        // ugh what happens if there are multiple loops?
        // like two squares
        /*

        S is this F here
        F--7
        |  |
        L--J--F--7
              |  |
              L--J
        oh maybe it cant happen because there are max 2 neighbors!
        */

        HashSet<Node> visited = new([animal]);
        Queue<(Node previous, Node current)> queue = new([(animal, animal)]);
        int steps = 0; 

        while (queue.Count > 0)
        {
            foreach (var _ in Enumerable.Range(0, queue.Count))
            {
                var (previous, current) = queue.Dequeue();

                foreach (var neighbor in GetConnectedNeighbors(current))
                {
                    if (neighbor == previous)
                    {
                        continue;
                    }
                    if (visited.Contains(neighbor))
                    {
                        loopSize = (steps + 1) * 2; // we find it halfway
                        return true;
                    }
                    visited.Add(neighbor);
                    queue.Enqueue((current, neighbor));
                }
            }
            steps++;
        }

        loopSize = 0;
        return false;
    }

    private int MaxDistanceFromAnimal(int loopSize)
    {
        return loopSize / 2;
    }

    [Description("How many steps along the loop does it take to get from the starting position to the point farthest from the starting position?")]
    public string SolvePartOne()
    {
        // find the loop where S is. how? we loop in the possibilities of S, so the possibilities
        // of its neighbor. during the loop, we need to consider S as the possible pipe we decided it to be now.
        char[] possiblePipes = ['J', 'L', '7', 'F', '|', '-'];
        foreach (char animalTentativePipe in possiblePipes)
        {
            var animalNeighbors = map.GetValueOrSetDefault(animal, new List<Node>());
            animalNeighbors.Clear();
            SetPipeNeighbors(animal, animalTentativePipe, maxRow, maxColumn);

            // gosh what if it contains other loops
            if (IsAnimalLoop(map, out int loopSize))
            {
                return MaxDistanceFromAnimal(loopSize).ToString();
            }
        }

        throw new Exception("No loop found");
    }

    [Description("")]
    public string SolvePartTwo() =>
        "";
}
