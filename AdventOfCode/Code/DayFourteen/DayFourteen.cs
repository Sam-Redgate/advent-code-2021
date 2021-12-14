using System.Text;

namespace Code;

public static class DayFourteen
{
    public static void Run()
    {
        var input = File.ReadLines("./Resources/DayFourteenTest.txt").ToArray();

        const int steps = 10;
        var template = input.First();
        var rules = input.Where(row => row.Contains("->")).ToDictionary(row => row.Split(' ')[0], row => row.Split(' ')[^1]);

        var polymerElements = CalculatePolymer(template, rules, steps).ToCharArray();

        var (leastCommonElementCount, mostCommonElementCount) = FindLeastAndMostCommonElement(polymerElements);
        
        Console.WriteLine($"Result: {mostCommonElementCount - leastCommonElementCount}");
    }

    private static (int, int) FindLeastAndMostCommonElement(char[] polymerElements)
    {
        var elementCounts = new Dictionary<char, int>();
        foreach (var element in polymerElements)
        {
            if (!elementCounts.ContainsKey(element)) elementCounts[element] = 1;
            else elementCounts[element] += 1;
        }

        var counts = elementCounts.Values.OrderBy(v => v).ToArray();
        return (counts[0], counts[^1]);
    }

    private static string CalculatePolymer(string template, Dictionary<string,string> rules, int steps)
    {

        while (steps-- > 0)
        {
            var builder = new StringBuilder(template[..1]);
            
            foreach (var pair in IteratePairs(template))
            {
                if (rules.ContainsKey(pair))
                {
                    builder.Append($"{rules[pair]}{pair.ToCharArray()[1]}");
                    continue;
                }

                builder.Append($"{pair.ToCharArray()[1]}");
            }

            template = builder.ToString();
        }

        return template;
    }

    private static IEnumerable<string> IteratePairs(string template)
    {
        for (var i = 0; i < template.Length - 1; i++)
        {
            yield return template[i..(i+2)];
        }
    }
}