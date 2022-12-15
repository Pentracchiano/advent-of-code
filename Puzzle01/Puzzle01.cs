using System.ComponentModel;

[Description("Calorie Counting")]
class Puzzle01 : PuzzleSolution
{
    private PriorityQueue<int, int> bestThree = new PriorityQueue<int, int>();

    public void Setup(string input)
    {
        int elf = 0;
        foreach (string entry in input!.Split('\n'))
        {
            if (entry == "")
            {
                if (this.bestThree.Count >= 3)
                {
                    this.bestThree.EnqueueDequeue(elf, elf);
                }
                else
                {
                    this.bestThree.Enqueue(elf, elf);
                }

                elf = 0;
                continue;
            }
            elf += int.Parse(entry);
        }
    }

    [Description("Find the Elf carrying the most Calories. How many total Calories is that Elf carrying?")]
    public string SolvePartOne() =>
        this.bestThree.Peek().ToString();

    [Description("Find the top three Elves carrying the most Calories. How many Calories are those Elves carrying in total?")]
    public string SolvePartTwo() =>
        this.bestThree.UnorderedItems.Sum(e => e.Element).ToString();
}
