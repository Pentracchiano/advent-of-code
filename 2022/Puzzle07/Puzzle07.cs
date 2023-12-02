namespace Advent2022;

using System.ComponentModel;

[Description("No Space Left On Device")]
class Puzzle07 : PuzzleSolution
{
    private enum FileType { DIR, FILE };
    private record FileInfo(FileType type, string name, Dictionary<string, FileInfo>? children, FileInfo? parent)
    {
        public ulong size { get; set; }
    }

    private FileInfo root = new(FileType.DIR, "/", new(), null);

    private void ParseTree(string input)
    {
        var currentDirectory = root;
        foreach (string line in Iterators.GetLines(input))
        {
            if (line.StartsWith("$ cd "))
            {
                var newDirectory = line.Substring(5);

                if (newDirectory == "..")
                {
                    currentDirectory = currentDirectory.parent!;
                    continue;
                }
                else if (newDirectory == "/")
                {
                    currentDirectory = root;
                    continue;
                }

                var newDirectoryContents = new Dictionary<string, FileInfo>();
                currentDirectory.children!.TryAdd(newDirectory, new(FileType.DIR, newDirectory, new(), currentDirectory));

                currentDirectory = currentDirectory.children[newDirectory];

                continue;
            }

            if (line == "$ ls")
            {
                continue;
            }

            if (line.StartsWith("dir "))
            {
                var directoryName = line.Substring(4);
                currentDirectory.children!.TryAdd(directoryName, new(FileType.DIR, directoryName, new(), currentDirectory));

                continue;
            }

            // only the file remains
            var fileInfo = line.Split(' ');
            ulong size = ulong.Parse(fileInfo[0]);
            string name = fileInfo[1];

            var file = new FileInfo(FileType.FILE, name, null, currentDirectory);
            file.size = size;
            currentDirectory.children!.Add(name, file);
        }
    }

    private void BuildSizes(FileInfo startingDir)
    {
        foreach (var entry in startingDir.children!.Values)
        {
            if (entry.type != FileType.FILE)
            {
                BuildSizes(entry);
            }

            startingDir.size += entry.size;
        }
    }

    private IEnumerable<FileInfo> Descendants(FileInfo node)
    {
        return node.children?.Values.Concat(
            node.children?.Values.SelectMany(c => Descendants(c)) ?? Enumerable.Empty<FileInfo>()
        ) ?? Enumerable.Empty<FileInfo>();
    }

    private void PrintTree(FileInfo startingDir, int depth = 0)
    {
        if (depth == 0)
        {
            Console.WriteLine($"{startingDir.name} (total: {startingDir.size}):");
        }

        foreach (var entry in startingDir.children!.Values)
        {
            for (int i = 0; i < depth; i++) Console.Write("    ");
            if (entry.type == FileType.FILE)
            {
                Console.WriteLine($"{entry.name} {entry.size}");
            }
            else
            {
                Console.WriteLine($"{entry.name} (total: entry.size):");
                PrintTree(entry, depth + 1);
            }
        }
    }

    public void Setup(string input)
    {
        ParseTree(input);
        BuildSizes(root);
    }

    [Description("Find all of the directories with a total size of at most 100000. What is the sum of the total sizes of those directories?")]
    public string SolvePartOne() =>
        Descendants(root)
        .Where(fi => fi.type == FileType.DIR)
        .Select(fi => fi.size)
        .Where(size => size <= 100000)
        .Aggregate((acc, current) => acc + current)
        .ToString();

    [Description("Find the smallest directory that, if deleted, would free up enough space on the filesystem to run the update. What is the total size of that directory?")]
    public string SolvePartTwo() =>
        Descendants(root)
        .Where(fi => fi.type == FileType.DIR)
        .Select(fi => fi.size)
        .Where(size => size >= 30000000 - (70000000 - root.size))
        .Min()
        .ToString();
}