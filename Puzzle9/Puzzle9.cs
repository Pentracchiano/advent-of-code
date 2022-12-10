using System.ComponentModel;

[Description("Rope Bridge")]
class Puzzle9 : PuzzleSolution
{
    private record Direction(int row, int col);
    private record Move(Direction direction, int steps);
    private record Position()
    {
        public int row { get; set; } = 0;
        public int col { get; set; } = 0;
    };

    private List<Move> moves = new();

    private Direction ReadDirection(char input)
    {
        return input switch
        {
            'R' => new(0, 1),
            'L' => new(0, -1),
            'D' => new(1, 0),
            'U' => new(-1, 0),
            _ => throw new ArgumentException()
        };
    }

    private int Distance(Position a, Position b)
    {
        return Math.Max(Math.Abs(a.row - b.row), Math.Abs(a.col - b.col));
    }

    private void AdvanceTail(Position head, Position tail)
    {
        // adjacent or in the same spot
        if (Distance(head, tail) < 2)
        {
            return;
        }

        tail.row += Math.Min(1, Math.Abs(head.row - tail.row)) * Math.Sign(head.row - tail.row);
        tail.col += Math.Min(1, Math.Abs(head.col - tail.col)) * Math.Sign(head.col - tail.col);
    }

    private void AdvanceHead(Move move, Position head)
    {
        head.row += move.direction.row;
        head.col += move.direction.col;
    }

    private int SimulateRopeUniqueTailVisits(int knots)
    {
        List<Position> rope = new(Enumerable.Range(0, knots).Select(_ => new Position()));

        var tailIndex = ^1;
        var visited = new HashSet<Position> { rope[tailIndex] };

        foreach (var move in moves)
        {
            for (int _ = 0; _ < move.steps; _++)
            {
                AdvanceHead(move, rope[0]);
                for (int i = 0; i < rope.Count - 1; i++)
                {
                    AdvanceTail(rope[i], rope[i + 1]);
                }

                visited.Add(rope[tailIndex]);
            }
        }

        return visited.Count;
    }

    public void Setup(string input) =>
        moves =
            Iterators
            .GetLines(input)
            .Select(line => new Move(ReadDirection(line[0]), int.Parse(line.AsSpan(2))))
            .ToList();

    [Description("Simulate your complete hypothetical series of motions. How many positions does the tail of the rope visit at least once?")]
    public string SolvePartOne() =>
        SimulateRopeUniqueTailVisits(2).ToString();

    [Description("Simulate your complete series of motions on a larger rope with ten knots. How many positions does the tail of the rope visit at least once?")]
    public string SolvePartTwo() =>
        SimulateRopeUniqueTailVisits(10).ToString();
}