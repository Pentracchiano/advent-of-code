using System.ComponentModel;

[Description("Rock Paper Scissors")]
class Puzzle2 : PuzzleSolution
{
    private enum GameResult : int
    {
        Loss = 0,
        Draw = 3,
        Win = 6
    }

    private enum Move
    {
        Rock,
        Paper,
        Scissors
    };

    private readonly Dictionary<Move, Dictionary<Move, GameResult>> winningRules = new Dictionary<Move, Dictionary<Move, GameResult>>
    {
      {Move.Rock, new Dictionary<Move, GameResult>
        {
          {Move.Rock, GameResult.Draw},
          {Move.Paper, GameResult.Win},
          {Move.Scissors, GameResult.Loss}
        }
      },
      {Move.Paper, new Dictionary<Move, GameResult>
        {
          {Move.Rock, GameResult.Loss},
          {Move.Paper, GameResult.Draw},
          {Move.Scissors, GameResult.Win}
        }
      },
      {Move.Scissors, new Dictionary<Move, GameResult>
        {
          {Move.Rock, GameResult.Win},
          {Move.Paper, GameResult.Loss},
          {Move.Scissors, GameResult.Draw}
        }
      },
    };

    private readonly Dictionary<Move, short> movePoints = new Dictionary<Move, short>
    {
      {Move.Rock, 1},
      {Move.Paper, 2},
      {Move.Scissors, 3}
    };

    private IEnumerable<string>? strategies;

    private (Move, Move) ParseLinePartOne(string line)
    {
        return (
          line switch { ['A', ..] => Move.Rock, ['B', ..] => Move.Paper, ['C', ..] => Move.Scissors, _ => throw new FormatException() },
          line switch { [.., 'X'] => Move.Rock, [.., 'Y'] => Move.Paper, [.., 'Z'] => Move.Scissors, _ => throw new FormatException() }
        );
    }

    private (Move, Move) ParseLinePartTwo(string line)
    {
        Move theirs = line switch { ['A', ..] => Move.Rock, ['B', ..] => Move.Paper, ['C', ..] => Move.Scissors, _ => throw new FormatException() };

        Move mine = (line, theirs) switch
        {
            ([.., 'Y'], _) => theirs,
            ([.., 'X'], Move.Rock) => Move.Scissors,
            ([.., 'X'], Move.Paper) => Move.Rock,
            ([.., 'X'], Move.Scissors) => Move.Paper,
            ([.., 'Z'], Move.Rock) => Move.Paper,
            ([.., 'Z'], Move.Paper) => Move.Scissors,
            ([.., 'Z'], Move.Scissors) => Move.Rock,
            _ => throw new FormatException()
        };

        return (theirs, mine);
    }

    private string Solve(Func<string, (Move, Move)> parser)
    {
        int totalScore = 0;
        foreach (string entry in this.strategies!)
        {
            (Move theirs, Move mine) = parser(entry);
            totalScore += this.movePoints[mine];
            totalScore += (int)this.winningRules[theirs][mine];
        }
        return totalScore.ToString();

    }

    public void Setup(string input) =>
        this.strategies = Iterators.GetLines(input!);

    [Description("What would your total score following the guide according to your interpretation of X, Y, Z?")]
    public string SolvePartOne() =>
        this.Solve(this.ParseLinePartOne);

    [Description("What would your total score following the guide according to the intended interpretation of X, Y, Z?")]
    public string SolvePartTwo() =>
        this.Solve(this.ParseLinePartTwo);
}
