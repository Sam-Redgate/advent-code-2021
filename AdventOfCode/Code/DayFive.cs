using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DayFive
{
    [Test]
    public static void Run()
    {
        var map = new GeothermalVentMap(File.ReadLines("./Resources/DayFiveInput.txt"));
        Console.Out.WriteLine(map);
        var overlappingPoints = map.CountOverlappingPoints();
        Console.Out.WriteLine($"Number of overlapping points: {overlappingPoints}");
        overlappingPoints.Should().Be(16793);
    }
}