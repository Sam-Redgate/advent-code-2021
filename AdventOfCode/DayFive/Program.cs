using System;
using System.IO;

namespace DayFive
{
    internal class Program
    {
        public static void Main()
        {
            var map = new GeothermalVentMap(File.ReadLines("./Resources/input.txt"));
            Console.Out.WriteLine(map);
            Console.Out.WriteLine($"Number of overlapping points: {map.CountOverlappingPoints()}");
        }
    }
}