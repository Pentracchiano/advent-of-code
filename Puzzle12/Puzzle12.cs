using System.ComponentModel;

[Description("Hill Climbing Algorithm")]
class Puzzle12 : PuzzleSolution
{
    private record Position(int row, int col);

    private Position? start;
    private Position? end;

    private char[][]? map;

    private Position[] directions = {
        new Position(0, 1),
        new Position(0, -1),
        new Position(1, 0),
        new Position(-1, 0)
    };

    private IEnumerable<Position> Neighbors(Position position)
    {
        var height = (int)map![position.row][position.col];
        foreach (var direction in directions)
        {
            var newPosition = new Position(position.row + direction.row, position.col + direction.col);
            if (
                0 <= newPosition.row && newPosition.row < map.Count()
                && 0 <= newPosition.col && newPosition.col < map[newPosition.row].Count()
                && height >= (int)map[newPosition.row][newPosition.col] - 1
            )
            {
                yield return newPosition;
            }
        }
    }

    public void Setup(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        map = new char[lines.Count()][];

        int i = 0;
        foreach (var line in lines)
        {
            map[i] = new char[lines[i].Count()];

            int j = 0;
            foreach (var character in line)
            {
                if (character == 'S')
                {
                    map[i][j] = 'a';
                    start = new Position(i, j);
                }
                else if (character == 'E')
                {
                    map[i][j] = 'z';
                    end = new Position(i, j);
                }
                else
                {
                    map[i][j] = character;
                }

                j++;
            }

            i++;
        }
    }

    private int ShortestPath(IEnumerable<Position> startingPoints)
    {
        HashSet<Position> visited = new(startingPoints);
        Queue<Position> queue = new(startingPoints);

        int steps = 0;

        while (queue.Count > 0)
        {
            int toDequeue = queue.Count;

            for (int _ = 0; _ < toDequeue; _++)
            {
                var position = queue.Dequeue();

                if (position == end!)
                {
                    return steps;
                }

                foreach (var neighbor in Neighbors(position))
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            steps++;
        }

        throw new ArgumentException("No path found.");
    }

    [Description("What is the fewest steps required to move from your current position to the location that should get the best signal?")]
    public string SolvePartOne() =>
        ShortestPath(new[] { start! }).ToString();

    [Description("What is the fewest steps required to move starting from any square with elevation a to the location that should get the best signal?")]
    public string SolvePartTwo() =>
        ShortestPath(
            from i in Enumerable.Range(0, map!.Count())
            from j in Enumerable.Range(0, map![i].Count())
            where map![i][j] == 'a'
            select new Position(i, j)
        ).ToString();
}