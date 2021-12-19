using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DayEight
{
    private record Entry(IEnumerable<string> Signals, IEnumerable<string> Values);

    [Test]
    public static void Run()
    {
        var entries = ParseValues(File.ReadLines("./Resources/DayEightInput.txt"));

        var sum = 0;

        foreach (var (signals, values) in entries)
        {
            var register = new SignalRegister();
            register.UpdateRegister(signals);
            sum += register.DecodeSequences(values);
        }

        Console.WriteLine(sum);
        sum.Should().Be(1068933);
    }

    private static IEnumerable<Entry> ParseValues(IEnumerable<string> inputs) => inputs.Select(ParseEntry);

    private static Entry ParseEntry(string input)
    {
        var tokens = input.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var strings = tokens[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var values = tokens[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new Entry(strings, values);
    }
}

public class SignalRegister
{
    private readonly Dictionary<int, char[]> _register;

    public SignalRegister()
    {
        _register = new Dictionary<int, char[]>
        {
            {0, Array.Empty<char>()},
            {1, Array.Empty<char>()},
            {2, Array.Empty<char>()},
            {3, Array.Empty<char>()},
            {4, Array.Empty<char>()},
            {5, Array.Empty<char>()},
            {6, Array.Empty<char>()},
            {7, Array.Empty<char>()},
            {8, Array.Empty<char>()},
            {9, Array.Empty<char>()},
        };
    }

    public void UpdateRegister(IEnumerable<string> input)
    {
        var sequence = input.Select(s => s.ToCharArray()).ToArray();
        foreach (var stringSequence in sequence)
        {
            UpdateUniqueDigits(stringSequence);
        }

        var middleAndTopLeft = _register[4].Except(_register[1]);

        var fiveLongs = sequence.Where(s => s.Length == 5).ToArray();
        var sixLongs = sequence.Where(s => s.Length == 6).ToArray();

        _register[3] = fiveLongs.First(s => !_register[1].Except(s).Any());
        _register[5] = fiveLongs.First(s => !middleAndTopLeft.Except(s).Any());
        _register[2] = fiveLongs.First(s =>
            !s.SequenceEqual(_register[3]) && !s.SequenceEqual(_register[5]));

        _register[9] = sixLongs.First(s => !_register[4].Except(s).Any());
        _register[6] = sixLongs.First(s =>
            !s.SequenceEqual(_register[9]) && !_register[5].Except(s).Any());
        _register[0] = sixLongs.First(s =>
            !s.SequenceEqual(_register[6]) && !s.SequenceEqual(_register[9]));
    }

    public int DecodeSequences(IEnumerable<string> input)
    {
        var builder = new StringBuilder();
        var inputArray = input.ToArray();

        foreach (var sequence in inputArray)
        {
            foreach (var (digit, signals) in _register)
            {
                if (signals.OrderBy(s => s).SequenceEqual(sequence.OrderBy(s => s)))
                {
                    builder.Append(digit);
                }
            }
        }

        return Convert.ToInt32(builder.ToString());
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append("Register:");
        foreach (var (digit, signal) in _register)
        {
            builder.Append($"\n\t{digit}: {string.Join(",", signal)}");
        }

        builder.Append('\n');

        return builder.ToString();
    }

    private void UpdateUniqueDigits(char[] strings)
    {
        var uniqueValue = strings.Length switch
        {
            2 => 1,
            3 => 7,
            4 => 4,
            7 => 8,
            _ => -1
        };

        if (uniqueValue == -1) return;

        _register[uniqueValue] = strings;
    }
}