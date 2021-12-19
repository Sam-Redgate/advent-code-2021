namespace Code;

public class CaveGraph
{
    private class Cave
    {
        public string Name { get; }
        public Cave[] AdjoiningCaves { get; set; }

        public Cave(string name)
        {
            Name = name;
            AdjoiningCaves = Array.Empty<Cave>();
        }
    }

    private readonly Cave[] _caves;
    private readonly string _start;
    private readonly string _end;

    private CaveGraph(Cave[] caves, string start, string end)
    {
        _caves = caves;
        _start = start;
        _end = end;
    }

    public int CountPaths()
    {
        var startingCave = _caves.First(cave => cave.Name == _start);

        return SearchPath(Array.Empty<Cave>(), startingCave, false);
    }

    private int SearchPath(Cave[] visitedCaves, Cave current, bool doubleVisited)
    {
        if (visitedCaves.Contains(current) && IsSmallCave(current) && doubleVisited)
        {
            return 0;
        }

        if (IsSmallCave(current) && visitedCaves.Contains(current))
        {
            doubleVisited = true;
        }

        visitedCaves = visitedCaves.Append(current).ToArray();

        if (current.Name == _end)
        {
            // Console.WriteLine(string.Join(',', visitedCaves.Select(cave => cave.Name)));
            return 1;
        }

        if (doubleVisited)
        {
            return current.AdjoiningCaves.Except(visitedCaves.Where(IsSmallCave))
                .Sum(nextCave => SearchPath(visitedCaves, nextCave, doubleVisited));
        }
        
        var remainingCaves = current.AdjoiningCaves.Except(new[] {_caves.First(cave => cave.Name == _start)});

        return remainingCaves.Sum(nextCave => SearchPath(visitedCaves, nextCave, doubleVisited));
    }

    private static bool IsSmallCave(Cave current)
    {
        return current.Name.ToLower() == current.Name;
    }

    // private bool CaveUnderVisitLimits(Cave cave, int smallCaveVisitLimit)
    // {
    //     var name = cave.Name;
    //     if (name == _start) return false; // never visit start again.
    //     if (name.ToLower() == name && cave.Visited > smallCaveVisitLimit) return false;
    //     return true;
    // }

    public static CaveGraph CreateCaveGraph(IEnumerable<string> input, string start, string end)
    {
        var tunnels = input.Select(ParseEdgeIntoTunnel).ToArray();

        var caveNames = tunnels.Select(tunnel => tunnel.Item1).Union(tunnels.Select(tunnel => tunnel.Item2));

        var caves = caveNames.Select(name => new Cave(name)).ToArray();

        return new CaveGraph(caves.Select(cave => PopulateAdjoiningCaves(cave, caves, tunnels)).ToArray(), start, end);
    }

    private static (string, string) ParseEdgeIntoTunnel(string edge)
    {
        var caveA = edge.Split('-')[0];
        var caveB = edge.Split('-')[1];
        return (caveA, caveB);
    }

    private static Cave PopulateAdjoiningCaves(Cave current, IEnumerable<Cave> caves,
        IEnumerable<(string, string)> tunnels)
    {
        var tunnelArray = tunnels.ToArray();
        var associatedTunnels = SelectAssociatedTunnels(current.Name, tunnelArray);
        var adjoiningCaveNames = SelectAdjoiningCaves(current.Name, associatedTunnels);

        current.AdjoiningCaves = adjoiningCaveNames.Select(name => caves.First(cave => cave.Name == name)).ToArray();

        return current;
    }

    private static IEnumerable<(string, string)> SelectAssociatedTunnels(string caveName,
        IEnumerable<(string, string)> tunnels) =>
        tunnels.Where(tunnel => tunnel.Item1 == caveName || tunnel.Item2 == caveName);

    private static IEnumerable<string> SelectAdjoiningCaves(string caveName,
        IEnumerable<(string, string)> associatedTunnels) =>
        associatedTunnels.Select(tunnel => tunnel.Item1.Equals(caveName) ? tunnel.Item2 : tunnel.Item1);
}