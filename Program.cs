class Runner
{
    static void Main(string[] args)
    {
        PuzzleSolution solver = new Calories();

        solver.Setup(File.ReadAllText(@"Puzzle1\input.txt"));

        var result = solver.SolvePartOne(null);
        Console.WriteLine($"Part one: {result}");
        result = solver.SolvePartTwo(null);
        Console.WriteLine($"Part two: {result}");
        solver = new RockPaperScissors();

        solver.Setup(File.ReadAllText(@"Puzzle2\input.txt"));

        result = solver.SolvePartOne(null);
        Console.WriteLine($"Part one: {result}");
        result = solver.SolvePartTwo(null);
        Console.WriteLine($"Part two: {result}");
    }
}