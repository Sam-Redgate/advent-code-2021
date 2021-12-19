using System.Text;

namespace Code;

public class GeothermalVentMap
{
    private readonly int[][] _map;

    private record CoOrdinate(int X, int Y);

    private record VentLine(CoOrdinate Start, CoOrdinate End);

    public GeothermalVentMap(IEnumerable<string> ventLines)
    {
        IEnumerable<VentLine> parsedLines = ParseVentLines(ventLines).ToArray();
        _map = PlotVents(parsedLines, BuildEmptyMap(parsedLines));
    }

    public int CountOverlappingPoints() => _map.Aggregate(0,
        (sum, row) => row.Aggregate(sum, (minorSum, cell) => cell > 1 ? minorSum + 1 : minorSum));

    private static int[][] PlotVents(IEnumerable<VentLine> ventLines, int[][] map)
    {
        IEnumerable<CoOrdinate> coordinates = Array.Empty<CoOrdinate>();
        foreach (var ventLine in ventLines)
        {
            coordinates = coordinates.Concat(CalculateLinePoints(ventLine));
        }

        foreach (var (x, y) in coordinates)
        {
            map[y][x] += 1;
        }

        return map;
    }

    private static CoOrdinate[] CalculateLinePoints(VentLine ventLine)
    {
        IEnumerable<CoOrdinate> coords = Array.Empty<CoOrdinate>();

        var xRange = Math.Abs(ventLine.Start.X - ventLine.End.X);
        var yRange = Math.Abs(ventLine.Start.Y - ventLine.End.Y);

        var xStart = ventLine.Start.X < ventLine.End.X ? ventLine.Start.X : ventLine.End.X;
        var yStart = ventLine.Start.Y < ventLine.End.Y ? ventLine.Start.Y : ventLine.End.Y;

        // Vertical
        if (xRange == 0)
        {
            coords = CalculateSteps(ventLine.Start.Y, ventLine.End.Y).Aggregate(coords, (current, yStep) => current.Append(new CoOrdinate(xStart, yStep)));
        }
        // Horizontal
        else if (yRange == 0)
        {
            coords = CalculateSteps(ventLine.Start.X, ventLine.End.X).Aggregate(coords, (current, xStep) => current.Append(new CoOrdinate(xStep, yStart)));
        }
        // Diagonal
        else
        {
            coords = CalculateSteps(ventLine.Start.X, ventLine.End.X)
                .Zip(CalculateSteps(ventLine.Start.Y, ventLine.End.Y), (x, y) => new CoOrdinate(x, y));
        }

        return coords.ToArray();
    }

    private static IEnumerable<int> CalculateSteps(int a, int b)
    {
        var direction = a > b ? -1 : 1;
        var current = a;

        if (direction > 0)
        {
            while (current <= b)
            {
                yield return current;
                current += direction;
            }
        }
        else
        {
            while (current >= b)
            {
                yield return current;
                current += direction;
            }
        }
    }

    private static int[][] BuildEmptyMap(IEnumerable<VentLine> ventLines)
    {
        var ventLinesArray = ventLines as VentLine[] ?? ventLines.ToArray();
        var x = ventLinesArray.Aggregate(0, (sum, line) => new[] {sum, line.Start.X, line.End.X}.Max()) + 1;
        var y = ventLinesArray.Aggregate(0, (sum, line) => new[] {sum, line.Start.Y, line.End.Y}.Max()) + 1;

        IEnumerable<int[]> map = Array.Empty<int[]>();
        for (var i = 0; i < y; i++)
        {
            map = map.Append(new int[x]);
        }

        return map.ToArray();
    }

    private IEnumerable<VentLine> ParseVentLines(IEnumerable<string> input)
    {
        return input.Select(ParseVentLine);
    }

    private VentLine ParseVentLine(string input)
    {
        var tokens = input.Split(' ');

        var start = tokens[0];
        var end = tokens[^1];

        return new VentLine(ParseCoOrdinate(start), ParseCoOrdinate(end));
    }

    private static CoOrdinate ParseCoOrdinate(string input)
    {
        var tokens = input.Split(',');
        return new CoOrdinate(Convert.ToInt32(tokens[0]), Convert.ToInt32(tokens[^1]));
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var row in _map)
        {
            foreach (var number in row)
            {
                result.Append($"{number,2}");
            }

            result.Append('\n');
        }

        return result.ToString();
    }
}