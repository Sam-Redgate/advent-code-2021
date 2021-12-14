namespace Code;

public static class DayFourteen
{
    public static void Run()
    {
        var input = File.ReadLines("./Resources/DayFourteenInput.txt").ToArray();

        const int steps = 40;
        var template = input.First();
        var rules = ParseRules(input);

        var (leastCommonElementCount, mostCommonElementCount) = FindLeastAndMostCommonElement(template, rules, steps);

        Console.WriteLine($"Result: {mostCommonElementCount - leastCommonElementCount}");
    }

    private static (ulong, ulong) FindLeastAndMostCommonElement(string template, Dictionary<string, char> rules,
        int steps)
    {
        var characterCounts = MapCharacters(template);
        var pairCounts = InitializePairCountFromTemplate(template);

        while (steps-- > 0)
        {
            // keep a record of all the new pairs created so we can apply them "simultaneously"
            var ruleOutput = new Dictionary<string, ulong>();
            
            foreach (var rule in rules.Keys.Where(rule => pairCounts.ContainsKey(rule) && pairCounts[rule] > 0))
            {
                var (leftPair, rightPair) = CreateRuleOutput(rule, rules[rule]);

                SumDictionaryElements(ruleOutput, leftPair, pairCounts, rule);

                SumDictionaryElements(ruleOutput, rightPair, pairCounts, rule);

                SumDictionaryElements(characterCounts, rules[rule], pairCounts, rule);

                pairCounts[rule] = 0;
            }

            pairCounts = AggregatePairs(pairCounts, ruleOutput);
        }

        var orderedCounts = characterCounts.Values.OrderBy(v => v).ToArray();
        
        return (orderedCounts[0], orderedCounts[^1]);
    }

    private static Dictionary<string, ulong> InitializePairCountFromTemplate(string template)
    {
        return MapElementsCountToDictionary(IteratePairs(template).ToArray());
    }

    private static Dictionary<char, ulong> MapCharacters(string input)
    {
        return MapElementsCountToDictionary(input.ToCharArray());
    }

    private static Dictionary<T, ulong> MapElementsCountToDictionary<T>(T[] elements) where T : IEquatable<T>
    {
        return elements.ToHashSet().ToDictionary(key => key, key => (ulong) elements.Count(key.Equals));
    }

    private static Dictionary<string, ulong> AggregatePairs(Dictionary<string, ulong> destination,
        Dictionary<string, ulong> source)
    {
        foreach (var pair in source.Keys)
        {
            SumDictionaryElements(destination, pair, source, pair);
        }

        return destination;
    }

    private static void SumDictionaryElements<T1, T2>(IDictionary<T1, ulong> destination, T1 destinationKey,
        IReadOnlyDictionary<T2, ulong> source, T2 sourceKey)
    {
        if (destination.ContainsKey(destinationKey)) destination[destinationKey] += source[sourceKey];
        else destination[destinationKey] = source[sourceKey];
    }

    private static (string, string) CreateRuleOutput(string pair, char result)
    {
        var left = pair[0];
        var right = pair[^1];
        return ($"{left}{result}", $"{result}{right}");
    }

    private static IEnumerable<string> IteratePairs(string template)
    {
        for (var i = 0; i < template.Length - 1; i++)
        {
            yield return template[i..(i + 2)];
        }
    }

    private static Dictionary<string, char> ParseRules(IEnumerable<string> input)
    {
        return input.Where(row => row.Contains("->"))
            .ToDictionary(row => row.Split(' ')[0], row => row.Split(' ')[^1].ToCharArray()[0]);
    }
}