public static class DictionaryExtensions
{
    public static TValue GetValueOrSetDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue) where TKey : notnull
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value;
        }
        dict[key] = defaultValue;
        return defaultValue;
    }
}