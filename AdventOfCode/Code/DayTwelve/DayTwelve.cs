namespace Code;

public class DayTwelve
{
    public static void Run()
    {
        var startValue = "start";
        var endValue = "end";
        var edges = ParseEdges(File.ReadAllLines("./Resources/DayTwelveTest.txt"));

        var tree = Tree<string>.BuildTree(startValue, edges, GetValidDuplicateFilter(), GetLeafValueCheck(endValue));
        
        tree.PrintTree();
        Console.WriteLine(tree.CountValue(endValue));
    }

    private static IEnumerable<(string, string)> ParseEdges(IEnumerable<string> input)
    {
        return input.Select(line => (line.Split('-')[0], line.Split('-')[1]));
    }

    private static Func<string, string, bool> GetValidDuplicateFilter()
    {
        return (left, right) => !left.Equals(right.ToLower());
    }

    private static Func<string, bool> GetLeafValueCheck(string endValue)
    {
        return value => value.Equals(endValue);
    }
}