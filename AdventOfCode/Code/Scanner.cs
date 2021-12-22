using System.Text;

namespace Code;

public class Scanner
{
    public IEnumerable<Coordinate3D> Beacons { get; }
    public Coordinate3D ScannerLocation { get; set; }

    public Scanner(IEnumerable<string> scanReport)
    {
        Beacons = ParseBeacons(scanReport);
        ScannerLocation = new Coordinate3D(0, 0, 0);
    }

    public Scanner(IEnumerable<Coordinate3D> beacons)
    {
        Beacons = beacons;
        ScannerLocation = new Coordinate3D(0, 0, 0);
    }

    private static IEnumerable<Coordinate3D> ParseBeacons(IEnumerable<string> scanReport)
    {
        return scanReport.Select(ParseCoordinate).ToArray();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var (x, y, z) in Beacons)
        {
            builder.Append($"{x},{y},{z}\n");
        }

        return builder.ToString();
    }

    private static Coordinate3D ParseCoordinate(string beaconCoordinates)
    {
        var tokens = beaconCoordinates.Split(',');
        return new Coordinate3D(Convert.ToInt32(tokens[0]), Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]));
    }
}