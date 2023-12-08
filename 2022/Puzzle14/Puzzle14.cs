using System.ComponentModel;


[Description("Regolith Reservoir")]
class Puzzle14 : PuzzleSolution
{
    private enum TileType { WALL, AIR, SAND };

    private class Tile: IComparable<Tile>, IEquatable<Tile>
    {
        public int RowIndex {get; init;}
        public TileType Type {get; init;}

        public Tile(int rowIndex, TileType type)
        {
            RowIndex = rowIndex;
            Type = type;
        }

        public override bool Equals(object? obj) => obj is Tile tile && Equals(tile);

        public override int GetHashCode() => RowIndex.GetHashCode();

        public int CompareTo(Tile? other) => RowIndex.CompareTo(other!.RowIndex);

        public bool Equals(Tile? other) => RowIndex == other!.RowIndex;
    }

    private class Column: IComparable<Column>, IEquatable<Column>
    {
        public int ColumnIndex {get; init;}
        public SortedSet<Tile> Tiles {get; init;}

        public Column(int columnIndex)
        {
            ColumnIndex = columnIndex;
            Tiles = new();
        }

        public override bool Equals(object? obj) => obj is Column row && Equals(row);

        public override int GetHashCode() => ColumnIndex.GetHashCode();

        public int CompareTo(Column? other) => ColumnIndex.CompareTo(other!.ColumnIndex);

        public bool Equals(Column? other) => ColumnIndex == other!.ColumnIndex;
    }

    private SortedSet<Column> map = new();

    public void Setup(string input)
    {
        var segments =
            Iterators
            .GetLines(input)
            .SelectMany(line => line.Split(" -> "))
            .SelectMany(pair => pair.Split(','))
            .Select(int.Parse)
            .Chunk(2);


        foreach (var (start, end) in segments.Pairwise())
        {
            for (int j = 0; j < Math.Abs(end[0] - start[0]) + 1; j++)
            {
                Column intermediateColumn = map.SetDefault(new Column(Math.Min(start[0], end[0]) + j));

                for (int i = 0; i < Math.Abs(end[1] - start[1]) + 1; i++)
                {
                    intermediateColumn.Tiles.SetDefault(new(Math.Min(start[1], end[1]) + i, TileType.WALL));
                }
            }

        }
    }

    [Description("How many units of sand come to rest before sand starts flowing into the abyss below?")]
    public string SolvePartOne()
    {
        var pouringStart = new Column(500);
        map.TryGetValue(pouringStart, out pouringStart);

        // find the lowest point in column 500
        pouringStart.Tiles.ToList().ForEach(Console.WriteLine);

        return pouringStart.Tiles.GetViewBetween(new Tile(0, TileType.AIR), null).First().ToString();
    }

    [Description("")]
    public string SolvePartTwo()
    {
        throw new NotImplementedException();
    }
}