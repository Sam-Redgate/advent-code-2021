using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DayThree
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine(Test());
            Console.WriteLine(CalculatePowerConsumption(File.ReadLines("./Resources/input.txt")));
        }

        private record PowerReadings(string Gamma, string Epsilon);

        private static bool Test()
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

            var result = CalculatePowerConsumption(input);

            return result == new PowerReadings("10110", "01001");
        }

        private static PowerReadings CalculatePowerConsumption(IEnumerable<string> input)
        {
            var bitArrays = ParseToBitArrays(input).ToArray();
            var width = bitArrays[0].Length;
            var gammaResults = new BitArray(width);
            var epsilonResults = new BitArray(width);

            for (var i = width - 1; i >= 0; i--)
            {
                var index = i;
                var trues = SumTrue(bitArrays.Select(ba => ba.Get(index)));
                var falses = SumFalse(bitArrays.Select(ba => ba.Get(index)));

                gammaResults[i] = trues > falses;
            }

            for (var i = width - 1; i >= 0; i--)
            {
                var index = i;
                var trues = SumTrue(bitArrays.Select(ba => ba.Get(index)));
                var falses = SumFalse(bitArrays.Select(ba => ba.Get(index)));

                epsilonResults[i] = trues < falses;
            }

            return new PowerReadings(WriteBitsToString(gammaResults), WriteBitsToString(epsilonResults));
        }

        private static string WriteBitsToString(BitArray bits)
        {
            var result = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    result = result + "1";
                }
                else
                {
                    result = result + "0";
                }
            }

            return result;
        }

        private static int SumTrue(IEnumerable<bool> bools) => bools.Aggregate(0, (i, b) => b ? i + 1 : i);
        private static int SumFalse(IEnumerable<bool> bools) => bools.Aggregate(0, (i, b) => b ? i : i + 1);

        private static IEnumerable<BitArray> ParseToBitArrays(IEnumerable<string> input) =>
            input.Select(ParseToBitArray);

        private static BitArray ParseToBitArray(string input)
        {
            var bools = new bool[input.Length];
            var tokens = input.ToCharArray();

            for (int i = 0; i < input.Length; i++)
            {
                if (tokens[i] == '1')
                {
                    bools[i] = true;
                }
                else
                {
                    bools[i] = false;
                }
            }

            return new BitArray(bools);
        }
    }
}