public static class SortedSetExtensions
{
    public static T SetDefault<T>(this SortedSet<T> set, T defaultValue)
    {
        if (!set.Contains(defaultValue))
        {
            set.Add(defaultValue);
        }

        T? handle;
        set.TryGetValue(defaultValue, out handle);

        return handle!;
    }
}