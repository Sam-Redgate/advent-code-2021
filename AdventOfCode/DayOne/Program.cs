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

        private static int CountDepthIncreases(IEnumerable<string> depthMeasurements) =>
            CountDepthIncreases(depthMeasurements.Select(int.Parse));

        private static int CountDepthIncreases(IEnumerable<int> depthMeasurements, int windowSize = 3)
        {
            var currentMeasurements = depthMeasurements as int[] ?? depthMeasurements.ToArray();

            var measurementWindows = BuildWindowValues(currentMeasurements, windowSize);

            var count = 0;
            int? previousMeasurement = int.MaxValue;

            foreach (var currentMeasurement in measurementWindows)
            {
                if (currentMeasurement > previousMeasurement)
                {
                    count++;
                }

                previousMeasurement = currentMeasurement;
            }

            return count;
        }

        private static IEnumerable<int> BuildWindowValues(IEnumerable<int> values, int windowSize)
        {
            IEnumerable<int> windows = Array.Empty<int>();

            var valuesArray = values as int[] ?? values.ToArray();
            if (valuesArray.Length < windowSize)
            {
                return windows;
            }

            for (var i = 0; i <= valuesArray.Length - windowSize; i++)
            {
                windows = windows.Append(valuesArray.Skip(i).Take(windowSize).Sum());
            }

            return windows;
        }
    }
}