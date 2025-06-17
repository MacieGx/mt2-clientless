namespace MetinClientless.Handlers;

public class PlayerPointsHandler : IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        return [0x0A, 0x98];
    }

}