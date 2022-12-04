class RucksackReorganization : PuzzleSolution
{
    private IList<(ISet<char> first, ISet<char> second)> rucksacks = new List<(ISet<char> first, ISet<char> second)>();

    private int GetValue(char item)
    {
        int asciiValue = (int) item;
        if (65 <= asciiValue  && asciiValue <= 90)
        {
            return asciiValue - (int) 'A' + 27;
        }
        return asciiValue - (int) 'a' + 1;
    }

    public void Setup(string? input)
    {
        foreach (string line in Iterators.GetLines(input!))
        {
            this.rucksacks.Add(
                (
                    new HashSet<char>(line.Take(line.Count() / 2)),
                    new HashSet<char>(line.Skip(line.Count() / 2))
                )
            );
        }
    }

    public string SolvePartOne(string? input)
    {
        int sum = 0;
        foreach ((var first, var second) in this.rucksacks) 
        {
            var intersection = first.Intersect(second).First();
            sum += this.GetValue(intersection);
        }
        return sum.ToString();
    }

    public string SolvePartTwo(string? input)
    {
        int sum = 0;
        int i = 1;
        IEnumerable<char>? intersection = null;
        foreach ((var first, var second) in this.rucksacks)
        {
            var union = first.Union(second);
            intersection = intersection?.Intersect(union) ?? union;

            if (i % 3 == 0)
            {
                sum += this.GetValue(intersection!.First());
                intersection = null;
            }

            i++;
        }

        return sum.ToString();
    }
}