using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DayOne
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine(CountDepthIncreases(File.ReadLines("./Resources/input.txt")));
        }

        private static int CountDepthIncreases(IEnumerable<string> depthMeasurements) => CountDepthIncreases(depthMeasurements.Select(int.Parse));

        private static int CountDepthIncreases(IEnumerable<int> depthMeasurements)
        {
            var currentMeasurements = depthMeasurements as int[] ?? depthMeasurements.ToArray();

            if (currentMeasurements.Length < 2)
            {
                return 0;
            }

            var count = 0;

            int? previousMeasurement = int.MaxValue;
            foreach (var currentMeasurement in currentMeasurements)
            {
                if (currentMeasurement > previousMeasurement)
                {
                    count++;
                }

                previousMeasurement = currentMeasurement;
            }

            return count;
        }
    }
}