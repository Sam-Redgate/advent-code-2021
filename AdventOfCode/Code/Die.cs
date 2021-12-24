namespace Code.Resources;

public static class Die
{
    public interface IDie
    {
        public uint Roll();
    }

    private class SequentialDie : IDie
    {
        private readonly uint _faces;
        private uint _next = 1;

        public SequentialDie(uint faces = 100)
        {
            _faces = faces;
        }

        public uint Roll()
        {
            if (_next > _faces) _next = 1;
            return _next++;
        }
    }

    public enum DieType
    {
        Sequential
    }

    public static IDie GetDie(uint faces, DieType type)
    {
        return type switch
        {
            DieType.Sequential => new SequentialDie(faces),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}