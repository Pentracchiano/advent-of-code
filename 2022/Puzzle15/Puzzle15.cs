using System.ComponentModel;
using System.Text.RegularExpressions;

[Description("Beacon Exclusion Zone")]
class Puzzle15 : PuzzleSolution
{
    record Position(int x, int y);

    List<(Position sensor, Position beacon)> positions = new();

    public void Setup(string input)
    {
        foreach (var line in Iterators.GetLines(input))
        {
            var numbers = Regex.Matches(line, @"-?\d+");
            var sensor = new Position(int.Parse(numbers[0].ValueSpan), int.Parse(numbers[1].ValueSpan));
            var beacon = new Position(int.Parse(numbers[2].ValueSpan), int.Parse(numbers[3].ValueSpan));

            positions.Add((sensor, beacon));
        }
    }

    [Description("In the row where y=2000000, how many positions cannot contain a beacon?")]
    public string SolvePartOne()
    {
        throw new NotImplementedException();
    }

    [Description("")]
    public string SolvePartTwo()
    {
        throw new NotImplementedException();
    }
}