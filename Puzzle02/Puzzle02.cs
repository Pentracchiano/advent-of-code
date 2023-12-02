namespace Advent2023;

using System.ComponentModel;
using Game = List<Dictionary<string, int>>;

[Description("Cube Conundrum")]
class Puzzle02 : PuzzleSolution
{
    List<(int id, Game game)> games = new ();

    public void Setup(string input)
    {
        foreach (var line in Iterators.GetLines(input))
        {
            var colonSplit = line.Split(':');
            games.Add((int.Parse(colonSplit[0].Split(' ')[1]), ParseGame(colonSplit[1])));
        }
    }

    private Game ParseGame(string gameString)
    {
        var game = new Game();
        foreach (var semicolonSplit in gameString.Split(';'))
        {
            var cubeCounts = new Dictionary<string, int>();
            foreach (var commaSplit in semicolonSplit.Split(','))
            {
                var trimmed = commaSplit.Trim();
                var spaceSplit = trimmed.Split(' ');
                var count = int.Parse(spaceSplit[0]);
                var color = spaceSplit[1];
                cubeCounts[color] = count;
            }
            game.Add(cubeCounts);
        }
        return game;
    }

    private bool IsGamePossible(Game game, Dictionary<string, int> bag) =>
        game.All(cubeCounts =>
            cubeCounts.All(pair =>
                bag.TryGetValue(pair.Key, out int count) && count >= pair.Value));

    private Dictionary<string, int> MinimumPossibleCubes(Game game) =>
        game
        .SelectMany(cubeCounts => cubeCounts)
        .GroupBy(pair => pair.Key)
        .ToDictionary(group => group.Key, group => group.Max(pair => pair.Value));

    private int PowerOfBag(Dictionary<string, int> bag) =>
        bag.Aggregate(1, (acc, pair) => acc * pair.Value);

    [Description("Determine which games would have been possible if the bag had been loaded with only 12 red cubes, 13 green cubes, and 14 blue cubes. What is the sum of the IDs of those games?")]
    public string SolvePartOne()
    {
        // after part2, could have been MinimumPossibleCubes <= constraints basically
        var constraints = new Dictionary<string, int> { { "red", 12 }, { "green", 13 }, { "blue", 14 } };
        return games
        .Where(game => IsGamePossible(game.game, constraints))
        .Sum(game => game.id)
        .ToString();
    }

    [Description("For each game, find the minimum set of cubes that must have been present. What is the sum of the power of these sets?")]
    public string SolvePartTwo() =>
        games
        .Select(game => game.game)
        .Select(MinimumPossibleCubes)
        .Select(PowerOfBag)
        .Sum()
        .ToString();
}