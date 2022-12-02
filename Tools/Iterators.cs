class Iterators
{
    public static IEnumerable<string> GetLines(string lines)
    {
        // maybe i can have fun with some yield and indexof in order to not produce the entire list in memory
        return lines.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }
}