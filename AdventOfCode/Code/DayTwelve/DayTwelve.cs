namespace Code;

public static class DayTwelve
{
    public static void Run()
    {
        var startValue = "start";
        var endValue = "end";
        
        Console.WriteLine(CaveGraph.CreateCaveGraph(File.ReadAllLines("./Resources/DayTwelveInput.txt"), startValue, endValue).CountPaths());
    }
}