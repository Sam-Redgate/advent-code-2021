namespace Code;

public static class DayTwelve
{
    public static void Run()
    {
        var startValue = "start";
        var endValue = "end";
        var edges = ParseEdges(File.ReadAllLines("./Resources/DayTwelveInput.txt"));

        var tree = PathTree.BuildTree(startValue, edges);
        
        Console.WriteLine(tree.CountValue(endValue));
    }

    private static IEnumerable<(string, string)> ParseEdges(IEnumerable<string> input)
    {
        return input.Select(line => (line.Split('-')[0], line.Split('-')[1]));
    }
}