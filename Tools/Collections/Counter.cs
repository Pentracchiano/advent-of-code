using System.Text;

class Counter<TKey> : Dictionary<TKey, ulong> where TKey : notnull
{
    public Counter(IEnumerable<TKey> collection)
    {
        foreach (var key in collection)
        {
            this[key]++;
        }
    }

    public Counter()
    {

    }

    public new ulong this[TKey key]
    {
        get
        {
            ulong count = 0;
            TryGetValue(key, out count);
            return count;
        }
        set
        {
            if (value <= 0)
            {
                Remove(key);
                return;
            }

            base[key] = value;
        }
    }

    public new string ToString()
    {
        StringBuilder builder = new("Counter(");
        builder.AppendJoin(", ", this);
        builder.Append(")");
        return builder.ToString();
    }
}