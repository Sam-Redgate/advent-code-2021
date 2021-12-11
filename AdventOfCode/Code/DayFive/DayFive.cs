namespace Code;

internal static class DayFive
{
    public static void Run()
    {
        var map = new GeothermalVentMap(File.ReadLines("./Resources/DayFiveInput.txt"));
        Console.Out.WriteLine(map);
        Console.Out.WriteLine($"Number of overlapping points: {map.CountOverlappingPoints()}");
    }
}