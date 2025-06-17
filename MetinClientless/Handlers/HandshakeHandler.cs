using MetinClientless.Packets;
using MetinClientless.Packets.Send;

namespace MetinClientless.Handlers;

public class HandshakeHandler : IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var packet = PacketGCHandshake.Read(data);
        Console.WriteLine($"Handshake received: {packet.dwHandshake} {packet.dwTime} {packet.lDelta}");
        return PacketCGHandshake.Serialize(packet, GameState.CurrentPhase == EPhase.PHASE_GAME);
    }
}