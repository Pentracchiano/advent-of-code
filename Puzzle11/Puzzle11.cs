using System.ComponentModel;

[Description("Monkey in the Middle")]
class Puzzle11 : PuzzleSolution
{
    private record Monkey(Queue<long> items, Func<long, long> operation, Func<long, int> test, int divisionTest);
    private List<Monkey> monkeys = new();

    private (Func<long, int>, int) ParseTest(string[] testInput)
    {
        int divisibleBy = int.Parse(testInput[0].AsSpan(21));
        int ifTrue = int.Parse(testInput[1].AsSpan(29));
        int ifFalse = int.Parse(testInput[2].AsSpan(30));

        return (worry => worry % divisibleBy == 0 ? ifTrue : ifFalse, divisibleBy);
    }

    private Func<long, long> ParseOperation(string operationInput)
    {
        Func<long, long, long> operation = operationInput[23] switch
        {
            '*' => (long first, long second) => first * second,
            '+' => (long first, long second) => first + second,
            _ => throw new FormatException()
        };

        int second;
        int.TryParse(operationInput.AsSpan(25), out second);
        return worry => operation(worry, second != 0 ? second : worry);
    }

    private Queue<long> ParseItems(string itemsInput) =>
        new(itemsInput.Substring(18).Split(", ").Select(long.Parse));

    public void Setup(string input)
    {
        foreach (var monkeyInput in Iterators.GetLines(input).Chunk(6))
        {
            (Func<long, int> test, int divisibleBy) = ParseTest(monkeyInput[3..]);
            monkeys.Add(new(
                items: ParseItems(monkeyInput[1]),
                operation: ParseOperation(monkeyInput[2]),
                test: test,
                divisionTest: divisibleBy
            ));
        }
    }

    private long GreatestCommonDivisor(long a, long b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return a == 0 ? b : a;
    }


    private long LeastCommonMultiple(long a, long b) =>
        (a * b) / GreatestCommonDivisor(a, b);

    private ulong MonkeyBusinessAfter(int rounds, bool divideByThree)
    {
        var monkeyInspections = new Counter<Monkey>();
        long leastCommonMultipleOfTests = 
            monkeys
            .Select(monkey => monkey.divisionTest) 
            .Aggregate(seed: 1L, (long lcm, int value) => LeastCommonMultiple(lcm, value));
        
        

        for (int round = 0; round < rounds; round++)
        {
            foreach (var monkey in monkeys)
            {
                int toDequeue = monkey.items.Count;
                for (int _ = 0; _ < toDequeue; _++)
                {
                    var item = monkey.items.Dequeue();
                    item = monkey.operation(item) % leastCommonMultipleOfTests;

                    if (divideByThree)
                    {
                        item /= 3;
                    }
                    var newMonkey = monkey.test(item);
                    monkeys[newMonkey].items.Enqueue(item);

                    monkeyInspections[monkey]++;
                }
            }
        }

        monkeyInspections.Values.OrderDescending().ToList().ForEach(Console.WriteLine);
        return monkeyInspections.Values.OrderDescending().Take(2).Aggregate((acc, value) => acc * value);
    }

    [Description("What is the level of monkey business after 20 rounds of stuff-slinging simian shenanigans?")]
    public string SolvePartOne() =>
        MonkeyBusinessAfter(20, true).ToString();

    [Description("Starting again from the initial state in your puzzle input, what is the level of monkey business after 10000 rounds?")]
    public string SolvePartTwo() =>
        MonkeyBusinessAfter(20, false).ToString();
}