namespace MetinClientless.Packets;

public class PacketCGExchange
{
    public static byte[] Accept()
    {
        return new BufferBuilder(14, true)
            .AddByte((byte)EClientToServer.HEADER_CG_EXCHANGE)
            .AddByte((byte)ExchangeSubHeaderCG.EXCHANGE_SUBHEADER_CG_ACCEPT)
            .Build();
    }
    
    public static byte[] Cancel()
    {
        return new BufferBuilder(14, true)
            .AddByte((byte)EClientToServer.HEADER_CG_EXCHANGE)
            .AddByte((byte)ExchangeSubHeaderCG.EXCHANGE_SUBHEADER_CG_CANCEL)
            .Build();
    }
    
    public static byte[] AddGold(uint amount)
    {
        return new BufferBuilder(14, true)
            .AddByte((byte)EClientToServer.HEADER_CG_EXCHANGE)
            .AddByte((byte)ExchangeSubHeaderCG.EXCHANGE_SUBHEADER_CG_ELK_ADD)
            .AddUInt32(amount)
            .Build();
    }
    
    public static byte[] AddItem(ushort itemPos, byte displayPos)
    {
        return new BufferBuilder(14, true)
            .AddByte((byte)EClientToServer.HEADER_CG_EXCHANGE)
            .AddByte((byte)ExchangeSubHeaderCG.EXCHANGE_SUBHEADER_CG_ITEM_ADD)
            .AddUInt32(0)
            .AddUInt32(0)
            .AddByte(displayPos)
            .AddBytes(ItemPos.Write(EWindowType.INVENTORY, itemPos))
            .Build();
    }
}

public enum ExchangeSubHeaderCG
{
EXCHANGE_SUBHEADER_CG_START,	/* arg1 == vid of target character */
EXCHANGE_SUBHEADER_CG_ITEM_ADD,	/* arg1 == position of item */
EXCHANGE_SUBHEADER_CG_ITEM_DEL,	/* arg1 == position of item */
EXCHANGE_SUBHEADER_CG_ELK_ADD,	/* arg1 == amount of gold */
EXCHANGE_SUBHEADER_CG_ACCEPT,	/* arg1 == not used */
EXCHANGE_SUBHEADER_CG_CANCEL,	/* arg1 == not used */
};