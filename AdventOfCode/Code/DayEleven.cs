using FluentAssertions;
using NUnit.Framework;

namespace Code;

public record Coordinate(int X, int Y);

public record Octopus(int Energy, bool HasFlashed);

public static class DayEleven
{
    [Test]
    public static void Run()
    {
        var octopusMap = ParseMap(File.ReadAllLines("./Resources/DayElevenInput.txt"));

        var daysToRun = 0;

        do
        {
            IncrementEnergyLevel(octopusMap);
            daysToRun++;
            Console.WriteLine($"\b{daysToRun} days");

        } while (octopusMap.Sum(pair => pair.Value.Energy) != 0);

        daysToRun.Should().Be(348);
    }

    private static Dictionary<Coordinate, Octopus> ParseMap(IEnumerable<string> input)
    {
        var map = new Dictionary<Coordinate, Octopus>();

        var rows = input.ToArray();
        for (var row = 0; row < rows.Length; row++)
        {
            var characters = rows[row].ToCharArray();
            for (var column = 0; column < rows[row].Length; column++)
            {
                map[new Coordinate(column, row)] = new Octopus(Convert.ToInt32(characters[column].ToString()), false);
            }
        }

        return map;
    }

    private static void IncrementEnergyLevel(Dictionary<Coordinate, Octopus> map)
    {
        foreach (var position in map.Keys)
        {
            map[position] = IncreaseEnergy(map, position) with { HasFlashed = false};
        }

        foreach (var position in map.Keys) CheckFlash(position, map);

        foreach (var (coordinate, octopus) in map)
        {
            if (octopus.HasFlashed)
            {
                map[coordinate] = octopus with { Energy = 0, HasFlashed = false};
            }
        }
    }

    private static int CheckFlash(Coordinate position, Dictionary<Coordinate, Octopus> map)
    {
        if (map[position].Energy <= 9 || map[position].HasFlashed) return 0;

        var neighbours = map.Where((pair, _) => FlashIsInRange(position, pair.Key)).ToArray();

        map[position] = map[position] with {HasFlashed = true};
        foreach (var (coordinate, _) in neighbours)
        {
            map[coordinate] = IncreaseEnergy(map, coordinate);
        }

        return neighbours.Aggregate(0, (sum, pair) => sum + CheckFlash(pair.Key, map)) + 1;
    }

    private static Octopus IncreaseEnergy(Dictionary<Coordinate, Octopus> map, Coordinate coordinate)
    {
        return map[coordinate] with {Energy = map[coordinate].Energy + 1};
    }

    private static bool FlashIsInRange(Coordinate position, Coordinate candidate)
    {
        if (Math.Abs(position.X - candidate.X) > 1) return false;
        if (Math.Abs(position.Y - candidate.Y) > 1) return false;
        return true;
    }
}