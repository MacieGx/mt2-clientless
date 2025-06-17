using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class PingHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        return new BufferBuilder(1, true)
            .AddByte((byte)EClientToServer.HEADER_CG_PONG)
            .Build();
    }
}