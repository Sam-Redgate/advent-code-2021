using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DaySeven
{
    internal static class Program
    {
        private static void Main()
        {
            var result = CalculateOptimalHorizontalPosition(
                ParseHorizontalPositions(File.ReadLines("./Resources/test.txt").FirstOrDefault()).ToArray());
            // var result = CalculateOptimalHorizontalPosition(
            //     ParseHorizontalPositions(File.ReadLines("./Resources/input.txt").FirstOrDefault()).ToArray());
            Console.WriteLine($"Optimal horizontal position is {result}");
        }

        private static IEnumerable<int> ParseHorizontalPositions(string input)
        {
            return input.Split(',').Select(s => Convert.ToInt32(s));
        }

        private static int CalculateOptimalHorizontalPosition(int[] positions)
        {
            var greatestHorizontalPosition = positions.Max();

            var sumManeuvers = new int[greatestHorizontalPosition + 1];

            sumManeuvers = positions
                .Aggregate(sumManeuvers, (current, position) => current
                    .Zip(CalculateAllManeuvers(position, greatestHorizontalPosition))
                    .Select(a => a.First + a.Second).ToArray());

            var cheapestManeuver = sumManeuvers.Min();

            foreach (var maneuver in sumManeuvers)
            {
                if (maneuver == cheapestManeuver) return maneuver;
            }

            return -1; // Shouldn't be reached.
        }

        private static int[] CalculateAllManeuvers(int position, int greatestHorizontalPosition)
        {
            var maneuvers = new int[greatestHorizontalPosition + 1];
            for (var i = 0; i < maneuvers.Length; i++)
            {
                maneuvers[i] = Math.Abs(position - i);
            }

            return maneuvers;
        }
    }
}