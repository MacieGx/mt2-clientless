using System.Buffers.Binary;

namespace MetinClientless.Packets.Send;

public class PacketCGLogin4
{
    public static byte[] Serialize()
    {
        return new BufferBuilder(5, true)
            .AddByte((byte)EClientToServer.HEADER_CG_LOGIN4)
            .AddUInt32(Configuration.GameServer.Version)
            .Build();
    }
}