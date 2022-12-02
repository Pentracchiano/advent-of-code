class Calories : PuzzleSolution
{
    private PriorityQueue<int, int> bestThree = new PriorityQueue<int, int>();

    public void Setup(string? input)
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

    public string SolvePartOne(string? input) =>
        this.bestThree.Peek().ToString();

    public string SolvePartTwo(string? input) =>
        this.bestThree.UnorderedItems.Sum(e => e.Element).ToString();
}
