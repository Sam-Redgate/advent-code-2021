using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DayFour
{
    internal static class Program
    {
        private record Game(IEnumerable<int> Numbers, IEnumerable<Board> Boards);
        
        public static void Main()
        {
            // PlayGame(File.ReadLines("./Resources/test.txt"));
            PlayGame(File.ReadLines("./Resources/input.txt"));
        }

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
}