using System.Collections.Immutable;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Code;

public class DayTwenty
{
    [Test]
    public void TestOne()
    {
        var lines = File.ReadLines("./Resources/DayTwentyTest.txt").ToArray();
        var algorithm = lines[0];
        var image = Image.CreateImage(lines[2..]);

        Console.WriteLine(image);

        image.EnhanceImage(algorithm, 2);

        Console.WriteLine(image);

        image.CountLitPixels().Should().Be(35);
    }

    [Test]
    public void StarOne()
    {
        var lines = File.ReadLines("./Resources/DayTwentyInput.txt").ToArray();
        var algorithm = lines[0];
        var image = Image.CreateImage(lines[2..]);

        image.EnhanceImage(algorithm, 2);

        Console.WriteLine(image);

        image.CountLitPixels().Should().Be(5179);
    }

    [Test]
    public void TestTwo()
    {
        var lines = File.ReadLines("./Resources/DayTwentyTest.txt").ToArray();
        var algorithm = lines[0];
        var image = Image.CreateImage(lines[2..]);

        Console.WriteLine(image);

        image.EnhanceImage(algorithm, 50);

        Console.WriteLine(image);

        image.CountLitPixels().Should().Be(3351);
    }

    [Test]
    public void StarTwo()
    {
        var lines = File.ReadLines("./Resources/DayTwentyInput.txt").ToArray();
        var algorithm = lines[0];
        var image = Image.CreateImage(lines[2..]);

        Console.WriteLine(image);

        image.EnhanceImage(algorithm, 50);

        Console.WriteLine(image);

        image.CountLitPixels().Should().Be(16112);
    }

    [Test]
    public void TestCreateImage()
    {
        var lines = File.ReadLines("./Resources/DayTwentyTest.txt");
        var coordinates = Image.CreateImage(lines.Skip(2)).LitPixels;
        coordinates.Should().Contain(new Coordinate(0, 0));
        coordinates.Should().Contain(new Coordinate(3, 0));
        coordinates.Should().Contain(new Coordinate(0, 1));
        coordinates.Should().Contain(new Coordinate(0, 2));
        coordinates.Should().Contain(new Coordinate(1, 2));
        coordinates.Should().Contain(new Coordinate(4, 2));
        coordinates.Should().Contain(new Coordinate(2, 3));
        coordinates.Should().Contain(new Coordinate(2, 4));
        coordinates.Should().Contain(new Coordinate(3, 4));
        coordinates.Should().Contain(new Coordinate(4, 4));
    }

    [Test]
    public void TestSumNeighbourValue()
    {
        var set = new HashSet<Coordinate>();
        Image.SumNeighbourValue(new Coordinate(1, 1), set).Should().Be(0);

        set.Add(new Coordinate(1, 1));
        Image.SumNeighbourValue(new Coordinate(1, 1), set).Should().Be(16);

        set.Add(new Coordinate(1, 0));
        set.Add(new Coordinate(1, 2));
        Image.SumNeighbourValue(new Coordinate(1, 1), set).Should().Be(146);

        set.Add(new Coordinate(0, 1));
        set.Add(new Coordinate(2, 1));
        Image.SumNeighbourValue(new Coordinate(1, 1), set).Should().Be(186);

        set.Add(new Coordinate(0, 0));
        set.Add(new Coordinate(2, 0));
        set.Add(new Coordinate(0, 2));
        set.Add(new Coordinate(2, 2));
        Image.SumNeighbourValue(new Coordinate(1, 1), set).Should().Be(511);
        Image.SumNeighbourValue(new Coordinate(0, 0), set).Should().Be(27);
        Image.SumNeighbourValue(new Coordinate(2, 2), set).Should().Be(432);
    }

    [Test]
    public void TestImageEnhancement()
    {
        var lines = File.ReadLines("./Resources/DayTwentyTest.txt").ToArray();
        var algorithm = lines[0];
        var image = Image.CreateImage(lines[2..]);

        image.EnhanceImage(algorithm, 2);

        image.CountLitPixels().Should().Be(35);
    }
}

public class Image
{
    public IReadOnlySet<Coordinate> LitPixels { get; private set; }
    private char _affinity;

    private Image(IReadOnlySet<Coordinate> litPixels)
    {
        LitPixels = litPixels;
        _affinity = '.';
    }

    public static Image CreateImage(IEnumerable<string> rows)
    {
        var coordinateSet = new HashSet<Coordinate>();

        foreach (var (row, y) in rows.Select((row, i) => (row, i)))
        {
            foreach (var (pixel, x) in row.ToCharArray().Select((pixel, i) => (pixel, i)))
            {
                if (pixel == '#')
                {
                    coordinateSet.Add(new Coordinate(x, y));
                }
            }
        }

        return new Image(coordinateSet.ToImmutableHashSet());
    }

    public void EnhanceImage(string algorithm, int iterations)
    {
        var imageNeedsTrimming = algorithm[0] == '#';

        while (iterations-- > 0)
        {
            LitPixels = EnhanceImage(LitPixels, algorithm);
            
            if (imageNeedsTrimming && iterations % 2 == 0) LitPixels = TrimImage(10);
        }

    }

    private IReadOnlySet<Coordinate> TrimImage(int borderSize)
    {
        var floorX = LitPixels.Min(coordinate => coordinate.X);
        var floorY = LitPixels.Min(coordinate => coordinate.Y);
        var ceilingX = LitPixels.Max(coordinate => coordinate.X);
        var ceilingY = LitPixels.Max(coordinate => coordinate.Y);

        var trimmedImage = new HashSet<Coordinate>();

        for (var y = floorY + borderSize; y <= ceilingY - borderSize; y++)
        {
            for (var x = floorX + borderSize; x <= ceilingX - borderSize; x++)
            {
                var coordinate = new Coordinate(x, y);
                if (LitPixels.Contains(coordinate)) trimmedImage.Add(coordinate);
            }
        }

        return trimmedImage;
    }

    private HashSet<Coordinate> EnhanceImage(IReadOnlySet<Coordinate> referencePixels, string algorithm)
    {
        var algorithmLookup = algorithm.ToCharArray();
        var floorX = referencePixels.Min(coordinate => coordinate.X) - 10;
        var floorY = referencePixels.Min(coordinate => coordinate.Y) - 10;
        var ceilingX = referencePixels.Max(coordinate => coordinate.X) + 10;
        var ceilingY = referencePixels.Max(coordinate => coordinate.Y) + 10;

        // if our affinity is for lit pixels, add in a border of them to simulate our infinite field.
        if (_affinity == '#')
            referencePixels = referencePixels.Concat(BuildBorder(ceilingX, ceilingY)).ToImmutableHashSet();

        var enhancedImage = new HashSet<Coordinate>();

        for (var y = floorY; y <= ceilingY; y++)
        {
            for (var x = floorX; x <= ceilingX; x++)
            {
                // border pixels aren't part of our image.
                if (x == floorX || x == ceilingX) continue;
                if (y == floorY || y == ceilingY) continue;

                var index = SumNeighbourValue(new Coordinate(x, y), referencePixels);
                if (algorithmLookup[index] == '#') enhancedImage.Add(new Coordinate(x, y));
            }
        }

        return enhancedImage;
    }

    private static IEnumerable<Coordinate> BuildBorder(int ceilingX, int ceilingY)
    {
        var xRange = Enumerable.Range(0, ceilingX).ToArray();
        var yRange = Enumerable.Range(0, ceilingY).ToArray();

        var xValues = xRange[..1].Concat(xRange[^2..]).ToArray();
        var yValues = yRange[..1].Concat(yRange[^2..]);

        foreach (var yValue in yValues)
        {
            foreach (var xValue in xValues)
            {
                yield return new Coordinate(xValue, yValue);
            }
        }
    }

    internal static uint SumNeighbourValue(Coordinate origin, IReadOnlySet<Coordinate> coordinates)
    {
        var builder = new StringBuilder();
        var (originX, originY) = origin;

        for (var y = originY - 1; y <= originY + 1; y++)
        {
            for (var x = originX - 1; x <= originX + 1; x++)
            {
                if (coordinates.Contains(new Coordinate(x, y)))
                {
                    builder.Append('1');
                    continue;
                }

                builder.Append('0');
            }
        }

        return Convert.ToUInt32(builder.ToString(), 2);
    }

    public int CountLitPixels()
    {
        var floorX = LitPixels.Min(coordinate => coordinate.X);
        var floorY = LitPixels.Min(coordinate => coordinate.Y);
        var ceilingX = LitPixels.Max(coordinate => coordinate.X);
        var ceilingY = LitPixels.Max(coordinate => coordinate.Y);
        // Ignore the outer border.
        return LitPixels.Count(p => p.X != floorX || p.Y != floorY || p.X != ceilingX || p.Y != ceilingY);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        var floorX = LitPixels.Min(coordinate => coordinate.X);
        var floorY = LitPixels.Min(coordinate => coordinate.Y);
        var ceilingX = LitPixels.Max(coordinate => coordinate.X);
        var ceilingY = LitPixels.Max(coordinate => coordinate.Y);

        var referencePixels = LitPixels;

        // if our affinity is for lit pixels, add in a border of them to simulate our infinite field.
        if (_affinity == '#')
            referencePixels = referencePixels.Concat(BuildBorder(ceilingX, ceilingY)).ToImmutableHashSet();

        for (var y = floorY; y <= ceilingY; y++)
        {
            for (var x = floorX; x <= ceilingX; x++)
            {
                if (referencePixels.Contains(new Coordinate(x, y)))
                {
                    builder.Append('#');
                    continue;
                }

                builder.Append('.');
            }

            builder.Append('\n');
        }

        return builder.ToString();
    }
}