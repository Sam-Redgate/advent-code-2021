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

        private record Position(int Horizontal, int Depth, int Aim);

        private record Heading(Direction Direction, int Distance);

        private static void Main()
        {
            Console.WriteLine($"Test result: {Test()}");
            Console.WriteLine(CalculateSumPosition(File.ReadLines("./Resources/input.txt")));
        }

        private static Position CalculateSumPosition(IEnumerable<string> input)
        {
            var headings = ParseHeadings(input).ToArray();

            return headings.Aggregate(new Position(0, 0, 0), UpdatePosition);
        }

        private static Position UpdatePosition(Position position, Heading heading)
        {
            return heading.Direction switch
            {
                Direction.Down => position with {Aim = position.Aim + heading.Distance},
                Direction.Up => position with {Aim = position.Aim - heading.Distance},
                Direction.Forward => position with
                {
                    Horizontal = position.Horizontal + heading.Distance,
                    Depth = position.Depth + (position.Aim * heading.Distance)
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

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

            return result.Horizontal * result.Depth == 900;
        }
    }
}