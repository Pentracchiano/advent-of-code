class Runner
{
    static void Main(string[] args)
    {
        // foreach (string puzzleDirectory in Directory.GetDirectories(".", "Puzzle*", SearchOption.TopDirectoryOnly))
        // {
        //     string solverPath = Path.Combine(puzzleDirectory, "Solution.cs");
        // }

        // gotta make this automatic!

        // PuzzleSolution solver = new Calories();
        // solver.Setup(File.ReadAllText(@"Puzzle1\input.txt"));

        // var result = solver.SolvePartOne(null);
        // Console.WriteLine($"Part one: {result}");
        // result = solver.SolvePartTwo(null);
        // Console.WriteLine($"Part two: {result}");

        // solver = new RockPaperScissors();
        // solver.Setup(File.ReadAllText(@"Puzzle2\input.txt"));

        // result = solver.SolvePartOne(null);
        // Console.WriteLine($"Part one: {result}");
        // result = solver.SolvePartTwo(null);
        // Console.WriteLine($"Part two: {result}");

        var solver = new CampCleanup();
        solver.Setup(File.ReadAllText(@"Puzzle4\input.txt"));

        var result = solver.SolvePartOne(null);
        Console.WriteLine($"Part one: {result}");
        result = solver.SolvePartTwo(null);
        Console.WriteLine($"Part two: {result}");
    }
}