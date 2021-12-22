using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Code;

public class DayNineteen
{
    [Test]
    public void TestOne()
    {
        var fullReport = File.ReadLines("./Resources/DayNineteenTest.txt").ToArray();
        var maps = new LinkedList<Scanner>();
        for (var i = 0; i < fullReport.Length; i++)
        {
            if (fullReport[i].StartsWith("---"))
            {
                maps.AddLast(new Scanner(fullReport.Skip(i + 1).TakeWhile(row => !string.IsNullOrWhiteSpace(row))));
            }
        }

        var uniqueCoordinates = new HashSet<Coordinate3D>(TranslateScanners(maps).SelectMany(x => x.Beacons)).Count;

        uniqueCoordinates.Should().Be(79);
    }

    [Test]
    public void StarOne()
    {
        var fullReport = File.ReadLines("./Resources/DayNineteenInput.txt").ToArray();
        var maps = new LinkedList<Scanner>();
        for (var i = 0; i < fullReport.Length; i++)
        {
            if (fullReport[i].StartsWith("---"))
            {
                maps.AddLast(new Scanner(fullReport.Skip(i + 1).TakeWhile(row => !string.IsNullOrWhiteSpace(row))));
            }
        }

        var uniqueCoordinates = new HashSet<Coordinate3D>(TranslateScanners(maps).SelectMany(x => x.Beacons)).Count;

        uniqueCoordinates.Should().Be(362);
        Console.WriteLine(TranslateScanners(maps));
    }

    [Test]
    public void TestTwo()
    {
        var fullReport = File.ReadLines("./Resources/DayNineteenTest.txt").ToArray();
        var maps = new LinkedList<Scanner>();
        for (var i = 0; i < fullReport.Length; i++)
        {
            if (fullReport[i].StartsWith("---"))
            {
                maps.AddLast(new Scanner(fullReport.Skip(i + 1).TakeWhile(row => !string.IsNullOrWhiteSpace(row))));
            }
        }

        var translatedScanners = TranslateScanners(maps).ToArray();

        var mostDistantScannersDistance = 0;
        foreach (var scanner in translatedScanners)
        {
            foreach (var otherScanner in translatedScanners)
            {
                var distance = CalculateManhattanDistance(scanner, otherScanner);
                if (distance > mostDistantScannersDistance) mostDistantScannersDistance = distance;
            }
        }

        mostDistantScannersDistance.Should().Be(3621);
    }

    [Test]
    public void StarTwo()
    {
        var fullReport = File.ReadLines("./Resources/DayNineteenInput.txt").ToArray();
        var maps = new LinkedList<Scanner>();
        for (var i = 0; i < fullReport.Length; i++)
        {
            if (fullReport[i].StartsWith("---"))
            {
                maps.AddLast(new Scanner(fullReport.Skip(i + 1).TakeWhile(row => !string.IsNullOrWhiteSpace(row))));
            }
        }

        var translatedScanners = TranslateScanners(maps).ToArray();

        var mostDistantScannersDistance = 0;
        foreach (var scanner in translatedScanners)
        {
            foreach (var otherScanner in translatedScanners)
            {
                var distance = CalculateManhattanDistance(scanner, otherScanner);
                if (distance > mostDistantScannersDistance) mostDistantScannersDistance = distance;
            }
        }

        Console.WriteLine(mostDistantScannersDistance);
        mostDistantScannersDistance.Should().Be(12204);
    }

    [Test]
    public void TestParsing()
    {
        var fullReport = File.ReadLines("./Resources/DayNineteenTest.txt").ToArray();
        var maps = new LinkedList<Scanner>();
        for (var i = 0; i < fullReport.Length; i++)
        {
            if (fullReport[i].StartsWith("---"))
            {
                maps.AddLast(new Scanner(fullReport.Skip(i + 1).TakeWhile(row => !string.IsNullOrWhiteSpace(row))));
            }
        }

        foreach (var (map, index) in maps.Select((map, i) => (map, i)))
        {
            Console.WriteLine($"Scanner {index} report:");
            Console.Write(map);
        }
    }

    [Test]
    public void TestCoordinateTranslator()
    {
        var report = new[]
        {
            "-1,-1,1",
            "-2,-2,2",
            "-3,-3,3",
            "-2,-3,1",
            "5,6,-4",
            "8,0,7"
        };
        var map = new Scanner(report);
        var builder = new StringBuilder();
        var count = 0;

        foreach (var beacons in CoordinateTranslator.EnumerateTranslations(map.Beacons))
        {
            builder.Append($"{new Scanner(beacons)}");
            count++;
        }

        Console.WriteLine(count);
        Console.WriteLine(builder.ToString());
    }

    private static Scanner? GetTranslatedBeacons(Scanner referenceScanner,
        Scanner candidateScanner)
    {
        var referenceBeaconArray = referenceScanner.Beacons.ToArray();
        var candidateBeaconArray = candidateScanner.Beacons.ToArray();

        foreach (var referenceBeacon in referenceBeaconArray)
        {
            foreach (var candidateBeacon in candidateBeaconArray)
            {
                var translatedScanner = TranslateScanner(referenceBeacon, candidateBeacon, candidateScanner);
                var translatedBeacons = translatedScanner.Beacons.ToArray();
                var overlappingBeacons = referenceBeaconArray.Intersect(translatedBeacons).ToArray();

                if (overlappingBeacons.Length >= 12) return translatedScanner;
            }
        }

        return null;
    }

    private static Scanner TranslateScanner(Coordinate3D reference, Coordinate3D candidate,
        Scanner candidateScanner)
    {
        var (x, y, z) = reference;
        var (xc, yc, zc) = candidate;
        var relativeX = x - xc;
        var relativeY = y - yc;
        var relativeZ = z - zc;

        return new Scanner(candidateScanner.Beacons.Select(beacon =>
            new Coordinate3D(beacon.X + relativeX, beacon.Y + relativeY, beacon.Z + relativeZ)))
        {
            ScannerLocation = new Coordinate3D(relativeX, relativeY, relativeZ)
        };
    }

    private static IEnumerable<Scanner> TranslateScanners(IEnumerable<Scanner> scannerResults)
    {
        var scanners = scannerResults.ToArray();
        var translatedScanners = new Dictionary<int, Scanner>
        {
            [0] = scanners[0]
        };

        while (translatedScanners.Count != scanners.Length)
        {
            for (var i = 0; i < scanners.Length; i++)
            {
                if (!translatedScanners.ContainsKey(i)) continue;

                var referenceScanner = translatedScanners[i];
                for (var j = 0; j < scanners.Length; j++)
                {
                    if (translatedScanners.ContainsKey(j)) continue;
                    foreach (var rotation in CoordinateTranslator.EnumerateTranslations(scanners[j].Beacons)
                                 .Select(r => r.ToArray()))
                    {
                        var rotatedScanner = new Scanner(rotation);
                        var translatedScanner = GetTranslatedBeacons(referenceScanner, rotatedScanner);
                        if (translatedScanner != null)
                        {
                            translatedScanners[j] = translatedScanner;
                        }
                    }
                }
            }
        }

        return translatedScanners.Values;
    }

    private static int CalculateManhattanDistance(Scanner one, Scanner two)
    {
        return (one.ScannerLocation.X - two.ScannerLocation.X) + (one.ScannerLocation.Y - two.ScannerLocation.Y) +
               (one.ScannerLocation.Z - two.ScannerLocation.Z);
    }
}