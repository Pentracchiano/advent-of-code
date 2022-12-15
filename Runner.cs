using System.ComponentModel;
using System.Diagnostics;

class Runner
{
    private record struct Puzzle(PuzzleSolution solution, string input);

    private static string GetDescription(System.Reflection.MemberInfo info)
    {
        return ((DescriptionAttribute)Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute))!).Description;
    }

    private static void AddPuzzle(List<Puzzle> list, int day)
    {
        var puzzleString = $"Puzzle{day:D2}";

        list.Add(new Puzzle(
            (PuzzleSolution)(Activator.CreateInstance(Type.GetType(puzzleString)
                ?? throw new ArgumentException($"Solver for day {day} is not loaded in this assembly."))
                    ?? throw new ArgumentException("Impossible to instantiate solver for day {day}.")),
            File.ReadAllText(Path.Combine(puzzleString, "input.txt"))
        ));
    }

    private static List<Puzzle> LoadPuzzles(int day = 0)
    {
        var puzzles = new List<Puzzle>();

        if (day != 0)
        {
            AddPuzzle(puzzles, day);
            return puzzles;
        }

        var integerComparison = Comparer<string>.Create(
            (string a, string b) =>
                Comparer<int>.Default.Compare(int.Parse(a.AsSpan().Slice(8)), int.Parse(b.AsSpan().Slice(8)))
        );

        foreach (string puzzleDirectory in Directory.GetDirectories(".", "Puzzle*", SearchOption.TopDirectoryOnly).Order(integerComparison))
        {
            int foundPuzzleDay = int.Parse(puzzleDirectory.AsSpan().Slice(8));
            AddPuzzle(puzzles, foundPuzzleDay);
        }

        return puzzles;
    }

    private static void Run(int day = 0)
    {
        var puzzles = LoadPuzzles(day);

        foreach ((PuzzleSolution solver, string input) in puzzles)
        {
            var puzzleName = solver.GetType().FullName;
            var puzzleDescription = GetDescription(solver.GetType());

            Console.WriteLine($"Running {puzzleName}: {puzzleDescription}.");

            var watch = Stopwatch.StartNew();
            solver.Setup(input);
            watch.Stop();

            Console.WriteLine($"Setup completed. Elapsed: {watch.ElapsedMilliseconds} ms.");

            var partOneDescription = GetDescription(solver.GetType().GetMethod("SolvePartOne")!);

            Console.WriteLine($"Part one. {partOneDescription}");

            watch = Stopwatch.StartNew();
            var result = solver.SolvePartOne();
            watch.Stop();

            Console.WriteLine($"Result: \"{result}\". Elapsed: {watch.ElapsedMilliseconds} ms.");

            var partTwoDescription = GetDescription(solver.GetType().GetMethod("SolvePartTwo")!);

            Console.WriteLine($"Part two. {partTwoDescription}");

            watch = Stopwatch.StartNew();
            result = solver.SolvePartTwo();
            watch.Stop();

            Console.WriteLine($"Result: \"{result}\". Elapsed: {watch.ElapsedMilliseconds} ms.");

            Console.WriteLine("");
        }
    }

    static void Main(string[] args)
    {
        int day = 0;
        string introduction = "No specific day specified. Running all available puzzles.";
        if (args.Count() > 0 && int.TryParse(args[0], out day))
        {
            introduction = $"Day {day} specified through command line.";
        }
        Console.WriteLine(introduction + "\n");

        Run(day);
    }
}