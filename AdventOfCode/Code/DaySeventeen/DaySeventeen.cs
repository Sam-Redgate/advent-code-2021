namespace Code;

public static class DaySeventeen
{
    public static void Run()
    {
        const string target = "target area: x=169..206, y=-108..-68";
        var yValues = target.Split(',', StringSplitOptions.RemoveEmptyEntries)[^1].Trim()[2..].Split("..");
        var firstYValue = Convert.ToInt32(yValues[0]);
        var secondYValue = Convert.ToInt32(yValues[1]);

        var lowestYValue = firstYValue < secondYValue ? firstYValue : secondYValue;
        var initialVelocity = 0 - lowestYValue - 1;

        var highestPoint = (initialVelocity * (initialVelocity + 1)) / 2;
        Console.WriteLine($"Highest point: {highestPoint}");
    }
}