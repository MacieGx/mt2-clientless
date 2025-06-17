namespace MetinClientless.Packets;

public struct PacketGCHandshake
{
    public byte Header;
    public uint dwHandshake;
    public uint dwTime;
    public int lDelta;
    
    public static PacketGCHandshake Read(byte[] buffer)
    {
        return new PacketGCHandshake
        {
            Header = buffer[0],
            dwHandshake = BitConverter.ToUInt32(buffer, 1),
            dwTime = BitConverter.ToUInt32(buffer, 5),
            lDelta = BitConverter.ToInt32(buffer, 9)
        };
    }
}