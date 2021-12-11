namespace Code;

public static class Runner
{
    private static readonly Dictionary<int, Action> Solutions = new()
    {
        {1, DayOne.Run},
        {2, DayTwo.Run},
        {3, DayThree.Run},
        {4, DayFour.Run},
        {5, DayFive.Run},
        {6, DaySix.Run},
        {7, DaySeven.Run},
        {8, DayEight.Run},
        {9, DayNine.Run},
        {10, DayTen.Run},
        {11, DayEleven.Run},
    };

    public static void Main(string[] args)
    {
        if (ArgsContainValidDay(args, out var day))
        {
            Solutions[day]();
            return;
        }
        
        Console.WriteLine($"Unknown day {args[0]}");
    }

    private static bool ArgsContainValidDay(IReadOnlyList<string> args, out int day)
    {
        day = 0;
        return args.Count == 1 && int.TryParse(args[0], out day) && Solutions.ContainsKey(day);
    }
}