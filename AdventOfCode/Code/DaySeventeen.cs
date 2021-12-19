using FluentAssertions;
using NUnit.Framework;

namespace Code;

public static class DaySeventeen
{
    private record Velocity(int X, int Y);

    [Test]
    public static void Run()
    {
        const string target = "target area: x=169..206, y=-108..-68";
        // const string target = "target area: x=20..30, y=-10..-5";
        var yValues = target.Split(',', StringSplitOptions.RemoveEmptyEntries)[^1].Trim()[2..].Split("..");
        var xValues = target.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim()[15..].Split("..");
        var firstYValue = Convert.ToInt32(yValues[0]);
        var secondYValue = Convert.ToInt32(yValues[1]);
        var firstXValue = Convert.ToInt32(xValues[0]);
        var secondXValue = Convert.ToInt32(xValues[1]);

        var lowestYValue = firstYValue < secondYValue ? firstYValue : secondYValue;
        var lowestXValue = firstXValue < secondXValue ? firstXValue : secondXValue;

        var highestYValue = firstYValue > secondYValue ? firstYValue : secondYValue;
        var highestXValue = firstXValue > secondXValue ? firstXValue : secondXValue;

        var velocities = CalculateDistinctFiringVelocities(new Coordinate(highestXValue, highestYValue),
            new Coordinate(lowestXValue, lowestYValue));

        var result = velocities.Count();
        Console.WriteLine($"Unique firing velocities: {result}");
        result.Should().Be(2576);
    }

    private static IEnumerable<Velocity> CalculateDistinctFiringVelocities(Coordinate greaterCorner,
        Coordinate lesserCorner)
    {
        return new HashSet<Velocity>(EnumerateValidFiringVelocities(greaterCorner, lesserCorner));
    }

    private static IEnumerable<Velocity> EnumerateValidFiringVelocities(Coordinate greaterCorner,
        Coordinate lesserCorner)
    {
        var testVelocities = EnumeratePotentialFiringVelocities(greaterCorner, lesserCorner);
        var boundsX = greaterCorner.X;
        var boundsY = lesserCorner.Y;

        foreach (var startingVelocity in testVelocities)
        {
            var (coordinate, velocity) = CalculateNextVelocity(new Coordinate(0, 0), startingVelocity);
            while (coordinate.X <= boundsX && coordinate.Y >= boundsY)
            {
                if (IsWithinTarget(coordinate, greaterCorner, lesserCorner))
                {
                    yield return startingVelocity;
                }

                (coordinate, velocity) = CalculateNextVelocity(coordinate, velocity);
            }
        }
    }

    private static bool IsWithinTarget(Coordinate projectile, Coordinate greaterCorner, Coordinate lesserCorner)
    {
        if (projectile.X > greaterCorner.X) return false;
        if (projectile.X < lesserCorner.X) return false;
        if (projectile.Y > greaterCorner.Y) return false;
        if (projectile.Y < lesserCorner.Y) return false;
        return true;
    }

    private static IEnumerable<Velocity> EnumeratePotentialFiringVelocities(Coordinate greaterCorner,
        Coordinate lesserCorner)
    {
        // A velocity with lower value than this can't possibly hit the target.
        var floorX = (int) Math.Sqrt(lesserCorner.X * 2) - 1;
        var floorY = lesserCorner.Y;

        // A velocity with higher value than this can't possibly hit the target.
        var ceilingX = greaterCorner.X;
        var ceilingY = Math.Abs(lesserCorner.Y);

        for (var x = floorX; x <= ceilingX; x++)
        {
            for (var y = floorY; y <= ceilingY; y++)
            {
                yield return new Velocity(x, y);
            }
        }
    }

    private static (Coordinate, Velocity) CalculateNextVelocity(Coordinate coordinate, Velocity velocity)
    {
        var nextVelocity = velocity with {X = velocity.X > 0 ? velocity.X - 1 : 0, Y = velocity.Y - 1};
        var nextCoordinate = coordinate with {X = coordinate.X + velocity.X, Y = coordinate.Y + velocity.Y};

        return (nextCoordinate, nextVelocity);
    }

    // private static IEnumerable<Coordinate> EnumerateCoordinates(Coordinate topLeft, Coordinate bottomRight)
    // {
    //     for (var x = topLeft.X; x < bottomRight.X; x++)
    //     {
    //         for (var y = topLeft.Y; y < bottomRight.Y; y++)
    //         {
    //             yield return new Coordinate(x, y);
    //         }
    //     }
    // }
}