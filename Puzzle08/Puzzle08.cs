using System.ComponentModel;

[Description("Treetop Tree House")]
class Puzzle08 : PuzzleSolution
{
    int[,] map = new int[0, 0];

    public void Setup(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        map = new int[lines.Count(), lines[0].Count()];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = int.Parse(lines[i].AsSpan(j, 1));
            }
        }
    }

    private IEnumerable<int> VisibleInDirection(int i, int j, int dimension, int step)
    {
        int value = map[i, j];

        int currentDimensionSize = dimension == 0 ? i : j;
        var ForwardsCondition = (int c) => c + currentDimensionSize < map.GetLength(dimension);
        var BackwardsCondition = (int c) => c + currentDimensionSize >= 0;

        var Condition = step > 0 ? ForwardsCondition : BackwardsCondition;

        var AddI = (int i, int j, int increment) => (i + increment, j);
        var AddJ = (int i, int j, int increment) => (i, j + increment);

        var AddDimension = dimension == 0 ? AddI : AddJ;

        for (int increment = step; Condition(increment); increment += step)
        {
            var (newI, newJ) = AddDimension(i, j, increment);
            if (value > map[newI, newJ]) yield return 1;
            else yield break;
        }
    }

    private bool IsVisible(int i, int j) =>
        VisibleInDirection(i, j, 0, 1).Sum() == map.GetLength(0) - i - 1
            || VisibleInDirection(i, j, 0, -1).Sum() == i
            || VisibleInDirection(i, j, 1, 1).Sum() == map.GetLength(1) - j - 1
            || VisibleInDirection(i, j, 1, -1).Sum() == j;


    private int ScenicScore(int i, int j)
    {
        int bottom = Math.Min(VisibleInDirection(i, j, 0, 1).Sum() + 1, map.GetLength(0) - i - 1);
        int top = Math.Min(VisibleInDirection(i, j, 0, -1).Sum() + 1, i);
        int right = Math.Min(VisibleInDirection(i, j, 1, 1).Sum() + 1, map.GetLength(1) - j - 1);
        int left = Math.Min(VisibleInDirection(i, j, 1, -1).Sum() + 1, j);

        return bottom * top * right * left;
    }

    [Description("Consider your map; how many trees are visible from outside the grid?")]
    public string SolvePartOne()
    {
        int visibleTrees = map.GetLength(0) * 2 + map.GetLength(1) * 2 - 4;

        for (int i = 1; i < map.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j++)
            {
                visibleTrees += Convert.ToInt32(IsVisible(i, j));
            }
        }

        return visibleTrees.ToString();
    }

    [Description("Consider each tree on your map. What is the highest scenic score possible for any tree?")]
    public string SolvePartTwo()
    {
        int maxVisibleTreesFromAnyTree = 0;

        for (int i = 1; i < map.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < map.GetLength(1) - 1; j++)
            {
                maxVisibleTreesFromAnyTree = Math.Max(
                    maxVisibleTreesFromAnyTree,
                    ScenicScore(i, j)
                );
            }
        }

        return maxVisibleTreesFromAnyTree.ToString();
    }
}