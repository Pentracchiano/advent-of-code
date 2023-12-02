namespace Advent2022;

using System.ComponentModel;

[Description("Tuning Trouble")]
class Puzzle06 : PuzzleSolution
{
    private string databuffer = string.Empty;

    private int FirstOccurrenceOfUniqueWindow(int windowSize)
    {
        Counter<char> letters = new();
        int processed = 0;

        foreach (char current in databuffer)
        {
            if (processed >= windowSize)
            {
                letters[databuffer[processed - windowSize]]--;
            }

            letters[current]++;
            processed++;

            if (letters.Count == windowSize)
            {
                return processed;
            }
        }

        return -1;
    }

    public void Setup(string input)
    {
        databuffer = input;
    }

    [Description("How many characters need to be processed before the first start-of-packet marker is detected?")]
    public string SolvePartOne() =>
        FirstOccurrenceOfUniqueWindow(4).ToString();

    [Description("How many characters need to be processed before the first start-of-message marker is detected?")]
    public string SolvePartTwo() =>
        FirstOccurrenceOfUniqueWindow(14).ToString();
}