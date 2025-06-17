namespace MetinClientless.Packets;

public struct ItemPos
{
    public EWindowType WindowType;
    public ushort Cell;
    
    public static ItemPos Read(byte[] buffer)
    {
        return new ItemPos
        {
            WindowType = (EWindowType) buffer[0],
            Cell = BitConverter.ToUInt16(buffer, 1)
        };
    }
    
    public static byte[] Write(EWindowType windowType, ushort cell)
    {
        return new BufferBuilder(3)
            .AddByte((byte) windowType)
            .AddUInt16(cell)
            .Build();
    }
}

public enum EWindowType
{
    RESERVED_WINDOW,
    INVENTORY,
    EQUIPMENT,
    SAFEBOX,
    MALL,
    DRAGON_SOUL_INVENTORY,
    AURA_REFINE,
    BELT_INVENTORY,
    GROUND,
    WINDOW_TYPE_MAX,
};