namespace MetinClientless.Packets;

public struct PacketGCExchange
{
    public EServerToClient Header;
    public ExchangeSubHeaderGC SubHeader;
    public bool IsMe;
    public ulong arg1;
    
    public static PacketGCExchange Read(byte[] buffer)
    {
        return new PacketGCExchange
        {
            Header = (EServerToClient) buffer[0],
            SubHeader = (ExchangeSubHeaderGC) buffer[1],
            IsMe = buffer[2] == 1,
            arg1 = BitConverter.ToUInt64(buffer, 3),
        };
    }
}

public enum ExchangeSubHeaderGC
{
    EXCHANGE_SUBHEADER_GC_START,
    EXCHANGE_SUBHEADER_GC_ITEM_ADD,
    EXCHANGE_SUBHEADER_GC_ITEM_DEL,
    EXCHANGE_SUBHEADER_GC_ELK_ADD,
    EXCHANGE_SUBHEADER_GC_ACCEPT,
    EXCHANGE_SUBHEADER_GC_END,
    EXCHANGE_SUBHEADER_GC_ALREADY,
    EXCHANGE_SUBHEADER_GC_LESS_ELK,
};