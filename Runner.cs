using System.ComponentModel;
using System.Diagnostics;

class Runner
{
    private record struct Puzzle(PuzzleSolution solution, string input);

    private static string GetDescription(System.Reflection.MemberInfo info)
    {
        return ((DescriptionAttribute)Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute))!).Description;
    }

    private static void AddPuzzle(List<Puzzle> list, int day, int year)
    {
        var puzzleName = $"Puzzle{day:D2}";
        var puzzleClassName = $"Advent{year}.{puzzleName}";

        list.Add(new Puzzle(
            (PuzzleSolution)(Activator.CreateInstance(Type.GetType(puzzleClassName)
                ?? throw new ArgumentException($"Solver for day {day} and year {year} is not loaded in this assembly."))
                    ?? throw new ArgumentException("Impossible to instantiate solver for day {day} and year {year}.")),
            File.ReadAllText(Path.Combine(puzzleName, "input.txt"))
        ));
    }

    private static List<Puzzle> LoadPuzzles(int year, int day = 0)
    {
        var puzzles = new List<Puzzle>();

        if (day != 0)
        {
            AddPuzzle(puzzles, day, year);
            return puzzles;
        }

        var integerComparison = Comparer<string>.Create(
            (string a, string b) =>
                Comparer<int>.Default.Compare(int.Parse(a.AsSpan().Slice(8)), int.Parse(b.AsSpan().Slice(8)))
        );

        foreach (string puzzleDirectory in Directory.GetDirectories(".", "Puzzle*", SearchOption.TopDirectoryOnly).Order(integerComparison))
        {
            int foundPuzzleDay = int.Parse(puzzleDirectory.AsSpan().Slice(8));
            AddPuzzle(puzzles, foundPuzzleDay, year);
        }

        return puzzles;
    }

    private static void Run(int year, int day = 0)
    {
        var puzzles = LoadPuzzles(year, day);

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
        int year = 2023;
        
        if (args.Length > 1 && int.TryParse(args[1], out int newYear))
        {
            year = newYear;
            Console.WriteLine($"Year {year} specified through command line.");
        }
        if (args.Length > 0 && int.TryParse(args[0], out day))
        {
            Console.WriteLine($"Day {day} specified through command line.");
        } else
        {
            Console.WriteLine($"No day specified through command line. Running all days for year {year}.");
        }
        Console.WriteLine();

        Run(year, day);
    }
}