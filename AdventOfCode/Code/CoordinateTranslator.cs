namespace Code;

public static class CoordinateTranslator
{
    public static IEnumerable<IEnumerable<Coordinate3D>> EnumerateTranslations(IEnumerable<Coordinate3D> coordinates)
    {
        return FaceAllDirections(coordinates).SelectMany(RotateAroundY);
    }

    private static IEnumerable<IEnumerable<Coordinate3D>> FaceAllDirections(IEnumerable<Coordinate3D> coordinates)
    {
        var coordinateArray = coordinates.ToArray();
        
        yield return coordinateArray;
        yield return coordinateArray.Select(coordinate => coordinate with {Y = coordinate.Z, Z = 0 - coordinate.Y});
        yield return coordinateArray.Select(coordinate => coordinate with {Y = 0 - coordinate.Y, Z = 0 - coordinate.Z});
        yield return coordinateArray.Select(coordinate => coordinate with {Y = 0 - coordinate.Z, Z = coordinate.Y});
        yield return coordinateArray.Select(coordinate => coordinate with {Y = coordinate.X, X = 0 - coordinate.Y});
        yield return coordinateArray.Select(coordinate => coordinate with {Y = 0 - coordinate.X, X = coordinate.Y});
    }

    private static IEnumerable<IEnumerable<Coordinate3D>> RotateAroundY(IEnumerable<Coordinate3D> coordinates)
    {
        var coordinateArray = coordinates.ToArray();
    
        yield return coordinateArray;
        yield return coordinateArray.Select(coordinate => coordinate with {X = coordinate.Z, Z = 0 - coordinate.X});
        yield return coordinateArray.Select(coordinate => coordinate with {X = 0 - coordinate.X, Z = 0 - coordinate.Z});
        yield return coordinateArray.Select(coordinate => coordinate with {X = 0 - coordinate.Z, Z = coordinate.X});
    }
}