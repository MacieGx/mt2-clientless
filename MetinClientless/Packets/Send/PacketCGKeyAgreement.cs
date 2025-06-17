using System.Buffers.Binary;

namespace MetinClientless.Packets.Send;

public class PacketCGKeyAgreement
{
    public static byte[] Serialize(byte[] key)
    {
        var buffer = new byte[261];
        var packetSpan = buffer.AsSpan();

        packetSpan[0] = (byte)EClientToServer.HEADER_CG_KEY_AGREEMENT;
        BinaryPrimitives.WriteUInt16LittleEndian(packetSpan[1..], 256);
        BinaryPrimitives.WriteUInt16LittleEndian(packetSpan[3..], 256);
        key.CopyTo(packetSpan[5..]);
        
        return buffer;
    }
}