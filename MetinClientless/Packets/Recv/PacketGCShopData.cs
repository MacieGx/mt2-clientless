using System.Text;

namespace MetinClientless.Packets;

public struct PacketGCShopData
{
    public uint Id;
    public ShopState State;
    
    public static PacketGCShopData Read(byte[] buffer)
    {
        return new PacketGCShopData
        {
            Id = BitConverter.ToUInt32(buffer, 6),
            State = (ShopState) buffer[5],
        };
    }
}

public enum ShopState {
    OPEN = 0x13,
    CLOSED = 0x14
}