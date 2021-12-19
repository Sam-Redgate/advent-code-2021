using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DayTen
{
    [Test]
    public static void Run()
    {
        IEnumerable<ulong> scores = Array.Empty<ulong>();
            
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var line in File.ReadLines("./Resources/DayTenInput.txt"))
        {
            var parser = SyntaxParser.ParseInput(line);
            if (parser.IsCorrupted)
            {
                continue;
            }

            scores = scores.Append(parser.AutoComplete());
        }

        scores = scores.OrderBy(score => score).ToArray();

        var result = scores.Skip(scores.Count() / 2).Take(1).First();
        Console.WriteLine(result);
        result.Should().Be(3646451424);
    }
}

public class SyntaxParser
{
    private readonly char[] _tokens;
    public bool IsCorrupted { get; }

    private SyntaxParser(bool isCorrupted, char[] tokens)
    {
        IsCorrupted = isCorrupted;
        _tokens = tokens;
    }

    public static SyntaxParser ParseInput(string input)
    {
        var chars = input.ToCharArray();
        var stack = new Stack<char>();
        var isCorrupted = false;

        foreach (var c in chars)
        {
            if (IsOpenCharacter(c))
            {
                stack.Push(c);
            }
            else if (IsCloseCharacter(c))
            {
                if (stack.Count > 0 && ArePair(stack.Peek(), c))
                {
                    stack.Pop();
                }
                else
                {
                    isCorrupted = true;
                    return new SyntaxParser(isCorrupted, stack.ToArray());
                }
            }
        }

        return new SyntaxParser(isCorrupted, stack.ToArray());
    }

    private static bool IsOpenCharacter(char c)
    {
        return c is '(' or '[' or '{' or '<';
    }

    private static bool IsCloseCharacter(char c)
    {
        return c is ')' or ']' or '}' or '>';
    }

    private static bool ArePair(char open, char close)
    {
        switch (open)
        {
            case '(' when close is ')':
            case '[' when close is ']':
            case '{' when close is '}':
            case '<' when close is '>':
                return true;
            default:
                return false;
        }
    }

    private static char GetClosingCharacter(char open)
    {
        return open switch
        {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            '<' => '>',
            _ => '0'
        };
    }

    private static ulong ScoreLegalCharacter(char c)
    {
        return c switch
        {
            ')' => 1ul,
            ']' => 2ul,
            '}' => 3ul,
            '>' => 4ul,
            _ => 0ul
        };
    }

    public ulong AutoComplete()
    {
        var looseOpeners = new Stack<char>();
        foreach (var c in _tokens)
        {
            if (IsOpenCharacter(c))
            {
                looseOpeners.Push(c);
            }
            else if (IsCloseCharacter(c))
            {
                looseOpeners.Pop();
            }
        }

        var listOfClosers = new char[looseOpeners.Count];

        for (var i = listOfClosers.Length - 1; i >= 0; i--)
        {
            listOfClosers[i] = GetClosingCharacter(looseOpeners.Pop());
        }

        var score = listOfClosers.Aggregate(0ul, (sum, closer) => sum * 5ul + ScoreLegalCharacter(closer));

        return score;
    }
}