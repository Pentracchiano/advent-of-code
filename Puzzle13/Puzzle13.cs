using System.ComponentModel;
using System.Text;

[Description("")]
class Puzzle13 : PuzzleSolution
{
    List<(PacketInfo left, PacketInfo right)> packetPairs = new();
    private enum PacketOrder { Undecided = 0, OutOfOrder = 1, InOrder = -1 };

    private class PacketInfo
    {
        private List<object> Values;

        public PacketInfo()
        {
            Values = new();
        }

        public IEnumerable<object> Contents
        {
            get
            {
                return Values.AsEnumerable();
            }
        }

        public int Count
        {
            get
            {
                return Values.Count;
            }
        }

        public void Add(object value)
        {
            Values.Add(value);
        }

        public override string ToString()
        {
            StringBuilder builder = new("[");
            builder.AppendJoin(',', Values);
            builder.Append("]");
            return builder.ToString();
        }

    }

    private IEnumerable<PacketInfo> DividerPackets
    {
        get
        {
            var dividerPackets = new List<PacketInfo>();

            var first = new PacketInfo();
            first.Add(new PacketInfo());
            ((PacketInfo)first.Contents.First()).Add(2);
            dividerPackets.Add(first);

            var second = new PacketInfo();
            second.Add(new PacketInfo());
            ((PacketInfo)second.Contents.First()).Add(6);
            dividerPackets.Add(second);

            return dividerPackets;
        }
    }

    private PacketInfo ParsePacket(string input)
    {
        Stack<PacketInfo> context = new();
        context.Push(new());  // i assume that a root list exists 

        int? current = null;
        var ConsumeInteger = () =>
        {
            if (current is not null)
            {
                context.Peek().Add(current);
                current = null;
            }
        };

        foreach (var character in input.Skip(1).SkipLast(1))  // i only consider what's inside []
        {
            switch (character)
            {
                case ',':
                    ConsumeInteger();
                    break;
                case '[':
                    context.Push(new());
                    break;
                case ']':
                    ConsumeInteger();
                    var list = context.Pop();
                    context.Peek().Add(list);
                    break;
                default:
                    current ??= 0;
                    current *= 10;
                    current += int.Parse(character.ToString());
                    break;
            }
        }

        ConsumeInteger();
        return context.Pop();
    }

    public void Setup(string input) => 
        packetPairs =
            Iterators
            .GetLines(input)
            .Chunk(2)
            .Select(pair => (ParsePacket(pair[0]), ParsePacket(pair[1])))
            .ToList();

    private PacketOrder AreInOrder((PacketInfo left, PacketInfo right) pair)
    {
        foreach (var (left, right) in pair.left.Contents.Zip(pair.right.Contents))
        {
            if (left is int leftInt && right is int rightInt)
            {
                if (rightInt != leftInt)
                {
                    return leftInt < rightInt ? PacketOrder.InOrder : PacketOrder.OutOfOrder;
                }
            }
            else if (left is PacketInfo leftPacket && right is PacketInfo rightPacket)
            {
                var subOrder = AreInOrder((leftPacket, rightPacket));
                if (subOrder != PacketOrder.Undecided)
                {
                    return subOrder;
                }
            }
            else if (left is int)
            {
                var wrapper = new PacketInfo();
                wrapper.Add(left);

                var subOrder = AreInOrder((wrapper, (PacketInfo)right));
                if (subOrder != PacketOrder.Undecided)
                {
                    return subOrder;
                }
            }
            else if (right is int)
            {
                var wrapper = new PacketInfo();
                wrapper.Add(right);

                var subOrder = AreInOrder(((PacketInfo)left, wrapper));
                if (subOrder != PacketOrder.Undecided)
                {
                    return subOrder;
                }
            }
        }

        if (pair.right.Count == pair.left.Count)
        {
            return PacketOrder.Undecided;
        }

        return pair.left.Count < pair.right.Count ? PacketOrder.InOrder : PacketOrder.OutOfOrder;
    }

    [Description("Determine which pairs of packets are already in the right order. What is the sum of the indices of those pairs?")]
    public string SolvePartOne() =>
        packetPairs
        .Select((packet, i) => (packet, i))
        .Where(indexedPair => AreInOrder(indexedPair.packet) == PacketOrder.InOrder)
        .Sum(indexedPair => indexedPair.i + 1)
        .ToString();

    [Description("Organize all of the packets into the correct order. What is the decoder key for the distress signal?")]
    public string SolvePartTwo() =>
        packetPairs
        .SelectMany(pair => new List<PacketInfo> { pair.left, pair.right })
        .Concat(DividerPackets)
        .Order(Comparer<PacketInfo>.Create((left, right) => (int)AreInOrder((left, right))))
        .Select((packet, i) => (packet, i))
        .Where(indexedPacket => DividerPackets.Any(divider => AreInOrder((divider, indexedPacket.packet)) == PacketOrder.Undecided))
        .Select(indexedPacket => indexedPacket.i + 1)
        .Aggregate((accumulation, current) => accumulation * current)
        .ToString();
}