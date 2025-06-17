namespace MetinClientless.Packets;

public interface IRecvPacket<T>
{
    public static int Size { get; }
    public static bool IsDynamic { get; }
    public static abstract T Read(byte[] buffer);
}