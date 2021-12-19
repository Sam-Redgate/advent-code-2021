#nullable enable
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DaySix
{
    [Test]
    public static void Run()
    { 
        var school = new LanternfishSchool(File.ReadLines("./Resources/DaySixInput.txt").First());
        const int daysToRun = 256;
            
        for (var daysRun = 0; daysToRun - daysRun > 0; daysRun++)
        {
            school.IncrementDay();
        }

        var lanternfish = school.CountFish();
        Console.WriteLine($"There are {lanternfish} lanternfish.");
        lanternfish.Should().Be(1741362314973u);
    }
}

public class LanternfishSchool
{
    private readonly ulong[] _school;

    public LanternfishSchool(string input)
    {
        _school = new ulong[9];
        foreach (var fish in ParseLanternfishInput(input))
        {
            _school[fish]++;
        }
    }

    public void IncrementDay()
    {
        var newFish = _school[0];

        Array.Copy(_school, 1, _school, 0, _school.Length - 1);

        _school[8] = newFish;
        _school[6] += newFish;
    }

    public ulong CountFish() => _school.Aggregate(0ul, (sum, fish) => sum + fish);

    private static IEnumerable<int> ParseLanternfishInput(string input) => input.Split(',').Select(s => Convert.ToInt32(s));

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var day in _school)
        {
            builder.Append($"{day},");
        }

        return builder.ToString();
    }
}