using System.ComponentModel;
using System.Text;

[Description("Cathode-Ray Tube")]
class Puzzle10 : PuzzleSolution
{
    private enum OperationType { ADDX, NOOP };
    private record Operation(OperationType Type, int Value);

    private List<Operation> operations = new();
    private List<int> registerValues = new();

    private Operation ParseLine(string line) =>
        line.Split(' ') switch
        {
            ["addx", var value] => new(OperationType.ADDX, int.Parse(value)),
            ["noop"] => new(OperationType.NOOP, 0),
            _ => throw new FormatException()
        };

    private void SimulateOperations()
    {
        int currentValue = 1;
        foreach (var operation in operations)
        {
            if (operation.Type == OperationType.ADDX)
            {
                registerValues.AddRange(Enumerable.Repeat(currentValue, 2));
                currentValue += operation.Value;
            }
            else if (operation.Type == OperationType.NOOP)
            {
                registerValues.Add(currentValue);
            }
        }
    }

    public void Setup(string input)
    {
        operations = Iterators
        .GetLines(input)
        .Select(ParseLine)
        .ToList();

        SimulateOperations();
    }


    [Description("Find the signal strength during the 20th, 60th, 100th, 140th, 180th, and 220th cycles. What is the sum of these six signal strengths?")]
    public string SolvePartOne() =>
        new List<int> { 20, 60, 100, 140, 180, 220 }
        .Select(i => registerValues[i - 1] * i)
        .Sum()
        .ToString();

    [Description("Render the image given by your program. What eight capital letters appear on your CRT?")]
    public string SolvePartTwo()
    {
        int Rows = 6;
        int Columns = 40;
        var crtScreen =  new StringBuilder();

        for (int cycle = 0; cycle < Rows * Columns; cycle++)
        {
            if (cycle % Columns == 0)
            {
                crtScreen.AppendLine();
            }

            crtScreen.Append(
                Enumerable
                .Range(-1, 3)
                .Select(i => registerValues[cycle] + i)
                .Contains(cycle % Columns) ? '#' : '.'
            );
        }

        return crtScreen.ToString();
    }
}