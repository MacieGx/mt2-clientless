namespace MetinClientless.Handlers;

public interface IPacketHandler
{
    int GetNewPort() => 0;
    Task<byte[]> HandlePacketAsync(byte[] data);
}

public struct PacketResponse
{
    public byte[] Data;
    public int NewPort;
}