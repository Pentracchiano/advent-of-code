namespace Advent2023;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Expressions;

[Description("Pipe Maze")]
class Puzzle10 : PuzzleSolution
{
    // ok so to understand the area inside of the loop, i can find the connected components of the graph
    // there will be 3 types: 
    // - the loop itself
    // - the area inside the loop
    // - the area outside the loop
    // this last one could actually be more than one area, if the loop is not connected to the border of the map.
    // so a way I can hack everything is to pad the map with a border of non-pipes, and then find the connected components.
    // so in this way, the external area will only be one, and can also be recognized as the one that contains a border node.

    private record Node(int row, int column);
    // what i built here is a directed graph.
    // in order for 2 pipes to be connected, they need to be neighbors on both sides.
    private Dictionary<Node, List<Node>> map = new();
    private Node animal = new(0, 0);
    int maxColumn = 0;
    int maxRow = 0;
    private string[] lines = new string[0];

    private void SetPipeNeighbors(Node node, char pipe, int maxRow, int maxColumn)
    {
        var (i, j) = node;
        var neighbors = map.GetValueOrSetDefault(node, new List<Node>());

        switch (pipe)
        {
            // i can actually remove all checks if the map is padded, but whatever
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
            case '.':
                break;
            default:
                throw new Exception("Unknown pipe");
        }
    }

    public void Setup(string input)
    {
        lines = input.Split(Environment.NewLine);
        lines = PadMap(lines, '.');
        (maxColumn, maxRow) = (lines[0].Length, lines.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
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

    private string[] PadMap(string[] lines, char pad)
    {
        // create a new array with 2 more rows and 2 more columns
        // fill it with the padding character
        var newLines = new string[lines.Length + 2];
        newLines[0] = new string(pad, lines[0].Length + 2);
        newLines[^1] = new string(pad, lines[0].Length + 2);
        for (int i = 1; i < newLines.Length - 1; i++)
        {
            for (int j = 0; j < lines[i - 1].Length; j++)
            {
                newLines[i] = pad + lines[i - 1] + pad;
            }
        }

        return newLines;
    }

    private IEnumerable<Node> GetConnectedNeighbors(Node node)
    {
        var neighbors = map.GetValueOrDefault(node, new List<Node>());
        return neighbors.Where(neighbor => map.GetValueOrDefault(neighbor, new List<Node>()).Contains(node));
    }

    private bool IsAnimalLoop(Dictionary<Node, List<Node>> map, [MaybeNullWhen(false)] out IEnumerable<Node> loop)
    {
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
                        loop = visited;
                        return true;
                    }
                    visited.Add(neighbor);
                    queue.Enqueue((current, neighbor));
                }
            }
            steps++;
        }

        loop = null;
        return false;
    }

    private int MaxDistanceFromAnimal(int loopSize)
    {
        return loopSize / 2;
    }

    private IEnumerable<Node> FindRealLoop()
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
            if (IsAnimalLoop(map, out var loop))
            {
                return loop;
            }
        }
        throw new Exception("No loop found");
    }

    private List<IEnumerable<Node>> ConnectedComponents(IEnumerable<Node> loop)
    {
        // the connected components are the ones that are either in the loop or not.
        // this time, we consider a 4-way connection, and use the map string directly.

        // so iterate over "lines" and as neighbors use
        var directions = new List<(int, int)>()
        {
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1),
        };

        HashSet<Node> visited = new();
        List<IEnumerable<Node>> components = new();

        for (int i = 0; i < maxRow; i++)
        {
            for (int j = 0; j < maxColumn; j++)
            {
                var node = new Node(i, j);
                if (visited.Contains(node))
                {
                    continue;
                }
                if (loop.Contains(node))
                {
                    continue;
                }

                // now we need to find the connected component of this node.
                // we can do it with a BFS.
                var component = new List<Node>();
                Queue<Node> queue = new();
                queue.Enqueue(node);
                visited.Add(node);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    component.Add(current);

                    foreach (var (di, dj) in directions)
                    {
                        if (current.row + di < 0 || current.row + di >= maxRow)
                        {
                            continue;
                        }
                        if (current.column + dj < 0 || current.column + dj >= maxColumn)
                        {
                            continue;
                        }

                        var neighbor = new Node(current.row + di, current.column + dj);
                        if (visited.Contains(neighbor))
                        {
                            continue;
                        }
                        if (loop.Contains(neighbor))
                        {
                            continue;
                        }

                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }

                components.Add(component);
            }
        }

        return components;
    }

    private bool IsInsideLoop(IEnumerable<Node> component, IEnumerable<Node> loop)
    {
        // turns out it wasn't needed to find the connected components.
        // or to check if the area is connected to the border.
        // some stuff can be outside the loop while technically being only connected to the loop
        // if the loop is encircling an area, but the area is not inside - it's merely encircled by the outside.
        // like this:

        /*
        The O here is actually OUTSIDE:
        ######################
        ##╔════╗╔╗╔╗╔╗╔═╗#####
        ##║╔══╗║║║║║║║║╔╝#####
        ##║║O╔╝║║║║║║║║╚╗#####
        #╔╝╚╗╚╗╚╝╚╝║║╚╝.╚═╗###
        #╚══╝#╚╗...╚╝S╗╔═╗╚╗##
        #####╔═╝..╔╗╔╝║╚╗╚╗╚╗#
        #####╚╗.╔╗║║╚╗║.╚╗╚╗║#
        ######║╔╝╚╝║╔╝║╔╗║#╚╝#
        #####╔╝╚═╗#║║#║║║║####
        #####╚═══╝#╚╝#╚╝╚╝####
        ######################
        
        so i could do some component analysis by going clockwise over the loop and checking left/right.
        what I am about to do, though, is for every component run a line - a ray - from one point to the end
        of the map. since there's a perimeter in the map, the number of times the ray crosses the perimeter
        is odd if the component is inside the loop, and even if it's outside.
        */
        var (i, j) = component.First();
        var timesCrossedLoop = 0;
        // let's arbitrarily go right
        while (j < maxColumn)
        {
            // if the two nodes are connected then we're on some kind of line,
            // we didn't cross the loop yet.
            var current = new Node(i, j);
            if (loop.Contains(current) && new[] { '7', 'F', '|' }.Contains(lines[current.row][current.column]))
            {
                timesCrossedLoop++;
            }
            j++;
        }
        return timesCrossedLoop % 2 == 1;
    }

    [Description("How many steps along the loop does it take to get from the starting position to the point farthest from the starting position?")]
    public string SolvePartOne() =>
        MaxDistanceFromAnimal(FindRealLoop().Count()).ToString();

    [Description("How many tiles are enclosed by the loop?")]
    public string SolvePartTwo()
    {
        var loop = FindRealLoop();

        // now we need to find the connected components of the graph, and remove the one that contains the border.
        // the border is the one that has a node with a row or column of 0 or maxRow or maxColumn.
        // The method does not return the loop as a component, and the connected components may be
        // of ground or pipes. we're only interested in the ground ones.

        // to debug, let's print the map with x as the loop,
        // . as the connectedcomponents inside the loop,
        // and # as the connectedcomponents outside the loop.

        var mapWithLoop = lines.Select(line => line.ToCharArray()).ToArray();
        foreach (var node in loop)
        {
            // let's have a better mapping for the visualization
            mapWithLoop[node.row][node.column] = lines[node.row][node.column] switch
            {
                'J' => '╝',
                'L' => '╚',
                '7' => '╗',
                'F' => '╔',
                '|' => '║',
                '-' => '═',
                'S' => 'S',
                _ => throw new Exception("Unknown pipe"),
            };
        }

        var components = ConnectedComponents(loop);
        foreach (var component in components)
        {
            var filler = '.';
            if (component.Any(c => c.row == 0 || c.row == maxRow - 1 || c.column == 0 || c.column == maxColumn - 1))
            {
                filler = '#';
            } else if (!IsInsideLoop(component, loop))
            {
                filler = 'O';
            }
            foreach (var node in component)
            {
                mapWithLoop[node.row][node.column] = filler;
            }
        }

        Console.WriteLine(string.Join(Environment.NewLine, mapWithLoop.Select(line => new string(line))));

        return ConnectedComponents(loop)
        .Where(component => !component.Any(node => node.row == 0 || node.row == maxRow - 1 || node.column == 0 || node.column == maxColumn - 1))
        .Select(component => component.Where(tile => lines[tile.row][tile.column] == '.'))
        .Where(component => IsInsideLoop(component, loop))
        .Sum(component => component.Count())
        .ToString();
    }
}