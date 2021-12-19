using FluentAssertions;
using NUnit.Framework;

namespace Code;

internal static class DayFour
{
    private record Game(IEnumerable<int> Numbers, IEnumerable<Board> Boards);

    [Test]
    public static void Run()
    {
        FindLastWinner(File.ReadLines("./Resources/DayFourInput.txt")).Should().Be(9576);
    }

    private static int FindLastWinner(IEnumerable<string> input)
    {
        var game = ParseGame(input);
        var winners = new Dictionary<string, Board>();

        Console.Write("Calling: ");
        foreach (var numberCalled in game.Numbers)
        {
            Console.Write($"{numberCalled},");
            foreach (var board in game.Boards)
            {
                var winner = board.MarkNumber(numberCalled);

                if (!winner) continue;

                winners[board.Id] = board;

                if (winners.Count != game.Boards.Count()) continue;

                Console.WriteLine(
                    $"Found final winner:\n\n{board}\nWinning value:{board.CalculateScore(numberCalled)}");
                return board.CalculateScore(numberCalled);
            }
        }

        return 0;
    }

    // ReSharper disable once UnusedMember.Local
    private static void PlayGame(IEnumerable<string> input)
    {
        var game = ParseGame(input);

        Console.Write("Calling: ");
        foreach (var numberCalled in game.Numbers)
        {
            Console.Write($"{numberCalled},");
            foreach (var board in game.Boards)
            {
                var winner = board.MarkNumber(numberCalled);

                if (!winner) continue;

                Console.WriteLine(
                    $"\nFound winner, winning board:\n\n{board}\nWinning value:{board.CalculateScore(numberCalled)}");
                return;
            }
        }
    }


    private static Game ParseGame(IEnumerable<string> input)
    {
        var inputArray = input.ToArray();

        var numbers = inputArray[0].Split(',').Select(s => Convert.ToInt32(s));

        var boards = Array.Empty<Board>();

        for (int i = 1; i < inputArray.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(inputArray[i]))
            {
                boards = boards.Append(new Board(inputArray.Skip(i + 1).Take(5))).ToArray();
            }
        }

        return new Game(numbers, boards);
    }
}