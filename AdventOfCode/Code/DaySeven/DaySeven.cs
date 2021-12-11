namespace Code;

internal static class DaySeven
{
    internal static void Run()
    {
        // var result = CalculateOptimalHorizontalPosition(
        //     ParseHorizontalPositions(File.ReadLines("./Resources/test.txt").FirstOrDefault()).ToArray());
        var result = CalculateOptimalHorizontalPosition(
            ParseHorizontalPositions(File.ReadLines("./Resources/DaySevenInput.txt").FirstOrDefault() ?? string.Empty).ToArray());
        Console.WriteLine($"Optimal horizontal position is {result}");
    }

    private static IEnumerable<int> ParseHorizontalPositions(string input)
    {
        return input.Split(',').Select(s => Convert.ToInt32(s));
    }

    private static int CalculateOptimalHorizontalPosition(int[] positions)
    {
        var greatestHorizontalPosition = positions.Max();

        var sumManeuvers = new int[greatestHorizontalPosition + 1];

        sumManeuvers = positions
            .Aggregate(sumManeuvers, (current, position) => current
                .Zip(CalculateAllManeuvers(position, greatestHorizontalPosition))
                .Select(a => a.First + a.Second).ToArray());

        return sumManeuvers.Min();
    }

    private static int[] CalculateAllManeuvers(int position, int greatestHorizontalPosition)
    {
        var maneuvers = new int[greatestHorizontalPosition + 1];
        for (var i = 0; i < maneuvers.Length; i++)
        {
            var horizontalOffset = position - i;

            var maneuverCost = horizontalOffset * (horizontalOffset + 1) / 2; // Good ol' Gauss
                
            maneuvers[i] = maneuverCost;
        }

        return maneuvers;
    }
}