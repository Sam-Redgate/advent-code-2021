using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DayTwo
{
    private enum Direction
    {
        Forward,
        Up,
        Down
    }

    private record Position(int Horizontal, int Depth, int Aim);

    private record Heading(Direction Direction, int Distance);

    [Test]
    public static void Run()
    {
        var position = CalculateSumPosition(File.ReadLines("./Resources/DayTwoInput.txt"));

        (position.Horizontal * position.Depth).Should().Be(1872757425);
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
            _ => throw new ArgumentOutOfRangeException(nameof(heading), $"heading.Direction of unexpected value {heading.Direction}")
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

    [Test]
    public static void Test()
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

        (result.Horizontal * result.Depth).Should().Be(900);
    }
}