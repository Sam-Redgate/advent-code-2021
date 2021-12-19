using FluentAssertions;
using NUnit.Framework;

namespace Code;

public static class DayFifteen
{
    [Test]
    public static void Run()
    {
        var page = ParseMap(File.ReadLines("./Resources/DayFifteenInput.txt").ToArray());
        var map = RebuildMapFromPages(page, 5, 5);

        var shortestCost =
            GetShortestPathCost(map, new Coordinate(0, 0), new Coordinate(map[0].Length - 1, map.Length - 1));
        Console.WriteLine(shortestCost);
        shortestCost.Should().Be(2952);
    }

    // Dijkstra, though it really should be A* I think, which should be quicker.
    private static int GetShortestPathCost(int[][] map, Coordinate start, Coordinate goal)
    {
        var costLookup = BuildCoordinateSetOfCosts(map);
        var remainingCoordinates = costLookup.ToDictionary(pair => pair.Key, _ => int.MaxValue);
        costLookup[start] = 0;
        remainingCoordinates[start] = 0;

        var coordinatesCostQueue = new PriorityQueue<Coordinate, int>(remainingCoordinates.Select(pair => (pair.Key, pair.Value)));

        while (remainingCoordinates.Count > 0)
        {
            var cheapestNode = coordinatesCostQueue.Dequeue();
            remainingCoordinates.Remove(cheapestNode);

            foreach (var neighbour in GetNeighbours(cheapestNode, remainingCoordinates))
            {
                var nextNodeCost = costLookup[cheapestNode] + costLookup[neighbour];

                if (nextNodeCost >= remainingCoordinates[neighbour]) continue;

                costLookup[neighbour] = nextNodeCost;
                remainingCoordinates[neighbour] = nextNodeCost;
                coordinatesCostQueue.Enqueue(neighbour, nextNodeCost);
            }
        }

        return costLookup[goal];
    }

    private static IEnumerable<Coordinate> GetNeighbours(Coordinate source, Dictionary<Coordinate, int> nodes)
    {
        var (x, y) = source;

        if (nodes.ContainsKey(new Coordinate(x, y - 1)))
            yield return new Coordinate(x, y - 1);

        if (nodes.ContainsKey(new Coordinate(x + 1, y)))
            yield return new Coordinate(x + 1, y);

        if (nodes.ContainsKey(new Coordinate(x, y + 1)))
            yield return new Coordinate(x, y + 1);

        if (nodes.ContainsKey(new Coordinate(x - 1, y)))
            yield return new Coordinate(x - 1, y);
    }

    private static Dictionary<Coordinate, int> BuildCoordinateSetOfCosts(int[][] grid)
    {
        var coordinateCosts = new Dictionary<Coordinate, int>();

        foreach (var coordinate in GridIterator(grid))
        {
            coordinateCosts[coordinate] = grid[coordinate.Y][coordinate.X];
        }

        return coordinateCosts;
    }

    private static int[][] RebuildMapFromPages(int[][] page, int pageHeight, int pageWidth)
    {
        var destination = new int[pageHeight * page.Length][];

        while (pageHeight-- > 0)
        {
            var yOffset = pageHeight * page.Length;
            for (var y = 0; y < page.Length; y++)
            {
                destination[y + yOffset] = CopyRow(page[y], pageHeight, pageWidth);
            }
        }

        return destination;
    }

    private static int[] CopyRow(IReadOnlyList<int> row, int offset, int width)
    {
        var destination = new int[row.Count * width];
        while (width-- > 0)
        {
            var xOffset = width * row.Count;
            for (var x = 0; x < row.Count; x++)
            {
                destination[x + xOffset] = AddRiskOffset(row[x], offset + width);
            }
        }

        return destination;
    }

    private static int AddRiskOffset(int value, int valueOffset)
    {
        var remainder = (value + valueOffset) % 9;
        return value + valueOffset > 9 ? remainder : value + valueOffset;
    }

    private static int[][] ParseMap(IReadOnlyList<string> rows)
    {
        var maxWidth = rows[0].Length;
        var maxHeight = rows.Count;
        var grid = new int[maxHeight][];

        for (var y = 0; y < maxHeight; y++)
        {
            grid[y] = new int[maxWidth];
            var row = rows[y].ToCharArray();
            for (var x = 0; x < maxWidth; x++)
            {
                grid[y][x] = Convert.ToInt32(row[x].ToString());
            }
        }

        return grid;
    }

    private static IEnumerable<Coordinate> GridIterator<T>(IReadOnlyList<T[]> page)
    {
        for (var y = 0; y < page.Count; y++)
        {
            for (var x = 0; x < page[y].Length; x++)
            {
                yield return new Coordinate(x, y);
            }
        }
    }
}