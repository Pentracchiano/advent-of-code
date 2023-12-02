namespace Advent2023;

using System.ComponentModel;

[Description("Trebuchet?!")]
class Puzzle01 : PuzzleSolution
{
    private string? input;
    private List<string> digitWords = new () 
    { 
        "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"
    };

    public void Setup(string input)
    {
        this.input = input;
    }

    [Description("Consider your entire calibration document. What is the sum of all of the calibration values?")]
    public string SolvePartOne()
    {
        int sum = 0;
        foreach (var line in Iterators.GetLines(input!))
        {
            var firstDigit = line.First(char.IsDigit);
            var lastDigit = line.Reverse().First(char.IsDigit);
            sum += int.Parse($"{firstDigit}{lastDigit}");
        }
        return sum.ToString();
    }

    private int ParseDigit(IEnumerable<char> line, IList<string> strings)
    {
        int[] indices = new int[strings.Count];
        foreach(var character in line)
        {
            if (char.IsDigit(character))
            {
                return character - '0';
            }
            for (int i = 0; i < strings.Count; i++)
            {
                if (character == strings[i][indices[i]])
                {
                    indices[i]++;
                    if (indices[i] == strings[i].Length)
                    {
                        return i + 1;
                    }
                }
                else if (character == strings[i][0])
                {
                    // Index should be reset but we already found a potential match start
                    indices[i] = 1;
                }
                else
                {
                    indices[i] = 0;
                }
            }
        }
        throw new Exception("No digit found");
    }

    private string Reverse(string input)
    {
        var array = input.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }
    
    [Description("It looks like some of the digits are actually spelled out with letters. What is the sum of all of the calibration values?")]
    public string SolvePartTwo()
    {
        int sum = 0;
        List<string> reversedDigitWords = digitWords.Select(x => Reverse(x)).ToList();
        foreach (var line in Iterators.GetLines(input!))
        {
            var first = ParseDigit(line, digitWords);
            var last = ParseDigit(line.Reverse(), reversedDigitWords);
            sum += first * 10 + last;
        }
        return sum.ToString();
    }
}