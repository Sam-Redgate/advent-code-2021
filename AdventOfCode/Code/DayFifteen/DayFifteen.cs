namespace Code;

public class DayFifteen
{
    public static void Run()
    {
        var map = ParseMap(File.ReadLines("./Resources/DayFifteenInput.txt").ToArray());
        var shortestCost = GetShortestPathCost(map, new Coordinate(0, 0), new Coordinate(map.GetLength(1) - 1, map.GetLength(0) - 1));
        Console.WriteLine(shortestCost);
    }

    // Dijkstra
    private static int GetShortestPathCost(int[,] map, Coordinate start, Coordinate goal)
    {
        var costLookup = BuildCoordinateSetOfCosts(map);
        costLookup[start] = 0;
        
        var remainingCoordinates = costLookup.ToDictionary(pair => pair.Key, _ => int.MaxValue);

        while (remainingCoordinates.Count > 0)
        {
            var cheapestNode = remainingCoordinates.MinBy(pair => pair.Value).Key;
            remainingCoordinates.Remove(cheapestNode);

            foreach (var neighbour in GetNeighbours(cheapestNode, remainingCoordinates))
            {
                var nextNodeCost = costLookup[cheapestNode] + costLookup[neighbour];
                
                if (nextNodeCost >= remainingCoordinates[neighbour]) continue;
                
                costLookup[neighbour] = nextNodeCost;
                remainingCoordinates[neighbour] = nextNodeCost;
            }
        }

        return costLookup[goal];
    }

    private static IEnumerable<Coordinate> GetNeighbours(Coordinate source, Dictionary<Coordinate,int> nodes)
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

    private static Dictionary<Coordinate, int> BuildCoordinateSetOfCosts(int[,] grid)
    {
        var coordinateCosts = new Dictionary<Coordinate, int>();

        foreach (var coordinate in GridIterator(grid))
        {
            coordinateCosts[coordinate] = grid[coordinate.Y, coordinate.X];
        }

        return coordinateCosts;
    }

    private static int[,] ParseMap(IReadOnlyList<string> rows)
    {
        var maxWidth = rows[0].Length;
        var maxHeight = rows.Count;
        var grid = new int[maxHeight, maxWidth];

        for (var y = 0; y < maxHeight; y++)
        {
            var row = rows[y].ToCharArray();
            for (var x = 0; x < maxWidth; x++)
            {
                grid[y, x] = Convert.ToInt32(row[x].ToString());
            }
        }

        return grid;
    }

    private static IEnumerable<Coordinate> GridIterator<T>(T[,] page)
    {
        var maxHeight = page.GetLength(0);
        var maxWidth = page.GetLength(1);

        for (var y = 0; y < maxHeight; y++)
        {
            for (var x = 0; x < maxWidth; x++)
            {
                yield return new Coordinate(x, y);
            }
        }
    }
}