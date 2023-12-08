static class Iterators
{
    public static IEnumerable<string> GetLines(string lines)
    {
        // maybe i can have fun with some yield and indexof in order to not produce the entire list in memory
        return lines.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> source)
    {
        return source.Zip(source.Skip(1));
    }

}