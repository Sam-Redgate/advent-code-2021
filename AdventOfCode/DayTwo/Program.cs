using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DayTwo
{
    internal static class Program
    {
        private enum Direction
        {
            Forward,
            Up,
            Down
        }

        private record Position(int Horizontal, int Depth);

        private record Heading(Direction Direction, int Distance);

        private static void Main()
        {
            Console.WriteLine($"Test result: {Test()}");
            Console.WriteLine(CalculateSumPosition(File.ReadLines("./Resources/input.txt")));
        }

        private static Position CalculateSumPosition(IEnumerable<string> input)
        {
            var headings = ParseHeadings(input).ToArray();

            var forward = SumHeadings(headings, Direction.Forward);
            var up = SumHeadings(headings, Direction.Up);
            var down = SumHeadings(headings, Direction.Down);

            return new Position(forward, down - up);
        }

        private static int SumHeadings(Heading[] headings, Direction direction) => headings
            .Where(h => h.Direction == direction)
            .Aggregate(0, (total, next) => total + next.Distance);

        private static IEnumerable<Heading> ParseHeadings(IEnumerable<string> headings) =>
            headings.Select(ParseHeading);

        private static Heading ParseHeading(string heading)
        {
            var tokens = heading.Split(' ');
            return new Heading(
                ParseDirection(tokens[0]),
                Int32.Parse(tokens[1])
            );
        }

        private static Direction ParseDirection(string direction) => Enum.Parse<Direction>(direction, true);

        private static bool Test()
        {
            var input = new[]
            {
                "forward 5",
                "down 5",
                "forward 8",
                "up 3",
                "down 8",
                "forward 2"
            };

            var result = CalculateSumPosition(input);

            return result.Horizontal * result.Depth == 150;
        }
    }
}