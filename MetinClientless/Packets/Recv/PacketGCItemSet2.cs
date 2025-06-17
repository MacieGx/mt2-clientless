namespace MetinClientless.Packets;

public struct PacketGCItemSet2
{
    public EServerToClient Header;
    public ItemPos InventoryPos;
    public uint ItemId;
    public uint Count;
    
    public static PacketGCItemSet2 Read(byte[] buffer)
    {
        return new PacketGCItemSet2
        {
            Header = (EServerToClient)buffer[0],
            InventoryPos = ItemPos.Read(buffer.AsSpan(1, 3).ToArray()),
            ItemId = BitConverter.ToUInt32(buffer, 4),
            Count = buffer[8]
        };
    }
}