#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaySix
{
    internal static class Program
    {
        private static void Main()
        { 
            Lanternfish[] lanternfish = ParseLanternfishInput(File.ReadLines("./Resources/input.txt").First()).ToArray();
            var daysToRun = 256;
            
            for (int daysRun = 0; daysToRun - daysRun > 0; daysRun++)
            {
                IEnumerable<Lanternfish> newFishList = Array.Empty<Lanternfish>();
                
                foreach (var fish in lanternfish)
                {
                    var newFish = fish.NewReproductionWindow();
                    if (newFish != null) newFishList = newFishList.Append(newFish);
                }

                lanternfish = lanternfish.Concat(newFishList).ToArray();
            }
            
            Console.WriteLine($"There are {lanternfish.Length} lanternfish.");
        }

        private static IEnumerable<Lanternfish> ParseLanternfishInput(string input) => input.Split(',').Select(s => Convert.ToInt32(s)).Select(t => new Lanternfish(t));
    }

    public class Lanternfish
    {
        private ushort _reproductionTimer;

        private Lanternfish()
        {
            _reproductionTimer = 8;
        }

        public Lanternfish(int reproductionTimer)
        {
            _reproductionTimer = Convert.ToUInt16(reproductionTimer);
        }

        public Lanternfish? NewReproductionWindow()
        {
            if (_reproductionTimer == 0)
            {
                _reproductionTimer = 6;
                return new Lanternfish();
            }
            _reproductionTimer--;
            return null;
        }

        public override string ToString()
        {
            return _reproductionTimer.ToString();
        }
        
    }
}