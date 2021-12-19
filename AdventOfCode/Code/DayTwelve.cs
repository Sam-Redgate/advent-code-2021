using FluentAssertions;
using NUnit.Framework;

namespace Code;

public static class DayTwelve
{
    [Test]
    public static void Run()
    {
        var startValue = "start";
        var endValue = "end";

        var pathCount = CaveGraph.CreateCaveGraph(File.ReadAllLines("./Resources/DayTwelveInput.txt"), startValue, endValue).CountPaths();
        Console.WriteLine(pathCount);
        pathCount.Should().Be(134862);
    }
}