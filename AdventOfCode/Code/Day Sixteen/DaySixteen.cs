namespace Code;

public static class DaySixteen
{
    private abstract record Packet(ushort Version, ushort Type);

    private record LiteralValuePacket(ushort Version, ushort Type, ulong Value) : Packet(Version, Type);

    private record OperatorPacket(ushort Version, ushort Type, char LengthType, IEnumerable<Packet> SubPackets): Packet(Version, Type);

    public static void Run()
    {
        var rawPacket = File.ReadAllLines("./Resources/DaySixteenInput.txt").First();
        var sum = GetVersionSum(rawPacket);

        Console.WriteLine($"Sum version numbers of all packets: {sum}");
    }

    private static int GetVersionSum(string rawPacket)
    {
        var (packet, _) = ParsePacket(ConvertHexStringToBinary(rawPacket));
        var sumVersion = GetVersionSum(packet);

        return sumVersion;
    }

    private static int GetVersionSum(Packet packet)
    {
        var sumVersion = (int)packet.Version;

        if (packet.Type == 4) return sumVersion;
        
        // packet is operator type.
        var opPacket = (OperatorPacket) packet;
        sumVersion += opPacket.SubPackets.Sum(GetVersionSum);

        return sumVersion;
    }

    private static (Packet, string) ParsePacket(string bits)
    {
        var version = bits[..3];
        var typeId = bits[3..6];
        var data = bits[6..];

        if (Convert.ToUInt16(typeId, 2) == 4)
        {
            return ParseLiteralValuePacket(Convert.ToUInt16(version, 2), Convert.ToUInt16(typeId, 2), data);
        }

        return ParseOperatorPacket(Convert.ToUInt16(version, 2), Convert.ToUInt16(typeId, 2), data);
    }

    private static string ConvertHexStringToBinary(string hex)
    {
        return string.Join(string.Empty,
            hex.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
    }

    private static (LiteralValuePacket, string) ParseLiteralValuePacket(ushort version, ushort type, string data)
    {
        var valueBits = string.Empty;
        foreach (var chunk in EnumerateLiteralValueChunks(data, 5))
        {
            valueBits = string.Concat(valueBits, chunk[1..]);
            data = data[5..];
            
            if (chunk[0] != '0') continue;
            break; // ignore remaining chunks
        }
        
        return (new LiteralValuePacket(version, type, Convert.ToUInt64(valueBits, 2)), data);
    }

    private static (OperatorPacket, string) ParseOperatorPacket(ushort version, ushort type, string data)
    {
        var lengthType = data[0];
        
        data = data[1..];

        IEnumerable<Packet> subPackets = Array.Empty<Packet>();
        
        if (lengthType == '0') // total length in bits
        {
            var totalLength = Convert.ToUInt32(data[..15], 2);
            data = data[15..];
            while (totalLength > 3)
            {
                var (packet, remainder) = ParsePacket(data);
                subPackets = subPackets.Append(packet);
                
                totalLength -= (uint)(data.Length - remainder.Length);
                data = remainder;
            }
        }
        else // number of sub-packets
        {
            var subPacketCount = Convert.ToUInt32(data[..11], 2);
            data = data[11..];
            while (subPacketCount > 0)
            {
                var (packet, remainder) = ParsePacket(data);
                subPackets = subPackets.Append(packet);
                data = remainder;
                
                subPacketCount--;
            }
        }

        return (new OperatorPacket(version, type, lengthType, subPackets), data);
    }

    private static IEnumerable<string> EnumerateLiteralValueChunks(string data, int chunkSize)
    {
        return Enumerable.Range(0, data.Length / chunkSize).Select(i => data.Substring(i * chunkSize, chunkSize));
    }
}