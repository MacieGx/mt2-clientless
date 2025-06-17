namespace MetinClientless.Packets;

public class PacketGCCharacterDel
{
    public uint Id;

    public static PacketGCCharacterDel Read(byte[] buffer)
    {
        return new PacketGCCharacterDel
        {
            Id = BitConverter.ToUInt32(buffer, 1)
        };
    }
}