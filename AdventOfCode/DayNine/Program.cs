namespace DayNine;

public static class Program
{
    public static void Main()
    {
        var map = new HeatMap(File.ReadLines("./Resources/input.txt"));
        Console.Out.WriteLine(map.Basins.OrderByDescending(b => b.Length).Take(3).Aggregate(1, (sum, b) => sum * b.Length));
    }
}

public class HeatMap
{
    public HeightPoint[][] Basins { get; }

    public record HeightPoint(int Height, bool Mapped = false);

    public HeatMap(IEnumerable<string> input)
    {
        Basins = CalculateBasins(ParseMap(input));
    }

    private static HeightPoint[][] CalculateBasins(HeightPoint[,] map)
    {
        var height = map.GetLength(0);
        var width = map.GetLength(1);
        var basins = Array.Empty<HeightPoint[]>();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (IsLowPoint(map[y, x]) && !map[y, x].Mapped)
                {
                    basins = basins.Append(BuildBasin(x, y, map)).ToArray();
                }
            }
        }

        return basins;
    }

    private static HeightPoint[] BuildBasin(int x, int y, HeightPoint[,] map)
    {
        var maxHeight = map.GetLength(0);
        var maxWidth = map.GetLength(1);
        
        if (!IsLowPoint(map[y, x]) || map[y, x].Mapped)
        {
            return Array.Empty<HeightPoint>();
        }

        map[y, x] = map[y, x] with {Mapped = true};

        var subBasin = new [] {map[y, x]};

        if (x - 1 >= 0) subBasin = subBasin.Concat(BuildBasin(x - 1, y, map)).ToArray();
        if (x + 1 < maxWidth) subBasin = subBasin.Concat(BuildBasin(x + 1, y, map)).ToArray();
        if (y - 1 >= 0) subBasin = subBasin.Concat(BuildBasin(x, y - 1, map)).ToArray();
        if (y + 1 < maxHeight) subBasin = subBasin.Concat(BuildBasin(x, y + 1, map)).ToArray();

        return subBasin;
    }

    private static HeightPoint[,] ParseMap(IEnumerable<string> rows)
    {
        var rowArray = rows.ToArray();

        var map = new HeightPoint[rowArray.Length, rowArray[0].Length];

        for (var y = 0; y < rowArray.Length; y++)
        {
            var heights = rowArray[y].ToCharArray().Select(c => Convert.ToInt32(c.ToString())).ToArray();
            for (var x = 0; x < rowArray[y].Length; x++)
            {
                map[y, x] = new HeightPoint(heights[x]);
            }
        }

        return map;
    }

    private static bool IsLowPoint(HeightPoint point) => point.Height < 9;
}