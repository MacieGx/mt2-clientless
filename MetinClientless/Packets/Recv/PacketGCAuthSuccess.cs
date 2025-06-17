namespace MetinClientless.Packets;

public struct PacketGCAuthSuccess
{
    public byte Header;
    public uint LoginKey;
    public byte Result;
    
    public static PacketGCAuthSuccess Read(byte[] buffer)
    {
        return new PacketGCAuthSuccess
        {
            Header = buffer[0],
            LoginKey = BitConverter.ToUInt32(buffer, 1),
            Result = buffer[5]
        };
    }
}