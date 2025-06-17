using System.Buffers.Binary;

namespace MetinClientless.Packets.Send;

public class PacketCGHandshake
{
    public static byte[] Serialize(PacketGCHandshake handshake, bool timeSync = false)
    {
        return new BufferBuilder(13, timeSync)
            .AddByte(timeSync ? (byte)EClientToServer.HEADER_CG_TIME_SYNC : (byte)EClientToServer.HEADER_CG_HANDSHAKE)
            .AddUInt32(handshake.dwHandshake)
            .AddUInt32(handshake.dwTime)
            .AddInt32(handshake.lDelta + 50)
            .Build();
    }
}