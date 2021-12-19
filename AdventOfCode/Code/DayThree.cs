using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DayThree
{
    [Test]
    public static void Run()
    {
        var input = File.ReadLines("./Resources/DayThreeInput.txt").ToArray();
        var oxygenRating = FindOxygenGeneratorRating(input);
        var co2Rating = FindCO2ScrubberRating(input);
        (Convert.ToUInt64(oxygenRating, 2) * Convert.ToUInt64(co2Rating, 2)).Should().Be(6940518);
    }

    [Test]
    public static void Test()
    {
        var input = new[]
        {
            "00100",
            "11110",
            "10110",
            "10111",
            "10101",
            "01111",
            "00111",
            "11100",
            "10000",
            "11001",
            "00010",
            "01010"
        };

        var oxygenResult = FindOxygenGeneratorRating(input);

        oxygenResult.Should().Be("10111");

        var co2Result = FindCO2ScrubberRating(input);
            
        co2Result.Should().Be("01010");
    }

    private static string FindOxygenGeneratorRating(string[] input, int index = 0)
    {
        switch (input.Length)
        {
            case 1:
                return input[0];
            case 2:
                return input[0].ToCharArray()[index] == '1' ? input[0] : input[1];
            default:
                return FindOxygenGeneratorRating(SelectGreatestOccurence(input, index), index + 1);
        }
    }

    private static string[] SelectGreatestOccurence(string[] input, int index = 0)
    {
        var zeroes = input.Where(i => i.ToCharArray()[index] == '0').ToArray();
        var ones = input.Where(i => i.ToCharArray()[index] == '1').ToArray();

        return zeroes.Length > ones.Length ? zeroes : ones;
    }

    // ReSharper disable once InconsistentNaming
    private static string FindCO2ScrubberRating(string[] input, int index = 0)
    {
        switch (input.Length)
        {
            case 1:
                return input[0];
            case 2:
                return input[0].ToCharArray()[index] == '0' ? input[0] : input[1];
            default:
                return FindCO2ScrubberRating(SelectLeastOccurence(input, index), index + 1);
        }
    }

    private static string[] SelectLeastOccurence(string[] input, int index = 0)
    {
        var zeroes = input.Where(i => i.ToCharArray()[index] == '0').ToArray();
        var ones = input.Where(i => i.ToCharArray()[index] == '1').ToArray();

        return zeroes.Length < ones.Length ? zeroes : ones;
    }
}