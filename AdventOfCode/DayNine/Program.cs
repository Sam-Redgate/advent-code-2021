namespace DayNine;

public static class Program
{
    public static void Main()
    {
        var map = ParseMap(File.ReadLines("./Resources/input.txt"));
        var result = CalculateLowestRiskSum(map);
        Console.WriteLine(result);
    }

    private static int CalculateLowestRiskSum(int[,] map)
    {
        var sum = 0;

        for (var y = 0; y < map.GetLength(0); y++)
        {
            for (var x = 0; x < map.GetLength(1); x++)
            {
                if (IsLowestPoint(x, y, map))
                {
                    sum += CalculateRiskLevel(map[y, x]);
                }
            }
        }

        return sum;
    }

    private static int[,] ParseMap(IEnumerable<string> rows)
    {
        var rowArray = rows.ToArray();

        var map = new int[rowArray.Length, rowArray[0].Length];

        for (var y = 0; y < rowArray.Length; y++)
        {
            var heights = rowArray[y].ToCharArray().Select(c => Convert.ToInt32(c.ToString())).ToArray();
            for (var x = 0; x < rowArray[y].Length; x++)
            {
                map[y, x] = heights[x];
            }
        }

        return map;
    }

    private static bool IsLowestPoint(int x, int y, int[,] map)
    {
        var maxHeight = map.GetLength(0);
        var maxWidth = map.GetLength(1);
        var currentValue = map[y, x];

        var isLowest = true;

        if (x - 1 >= 0) isLowest &= map[y, x - 1] > currentValue;
        if (x + 1 < maxWidth) isLowest &= map[y, x + 1] > currentValue;
        if (y - 1 >= 0) isLowest &= map[y - 1, x] > currentValue;
        if (y + 1 < maxHeight) isLowest &= map[y + 1, x] > currentValue;
        
        return isLowest;
    }

    private static int CalculateRiskLevel(int height) => height + 1;
}