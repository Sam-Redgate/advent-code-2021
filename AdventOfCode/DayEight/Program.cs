namespace DayEight
{
    internal static class Program
    {
        public static void Main()
        {
            Console.WriteLine(ParseValues(File.ReadLines("./Resources/input.txt")).Aggregate(0, (sum, values) => sum + CountUniqueDigits(values.Split())));
        }

        private static IEnumerable<string> ParseValues(IEnumerable<string> inputs)
        {
            return inputs.Select(i => i.Split('|')[1].Trim());
        }

        private static int CountUniqueDigits(IEnumerable<string> inputs)
        {
            return inputs.Count(i => i.Length is 2 or 4 or 3 or 7);
        }
    }
}