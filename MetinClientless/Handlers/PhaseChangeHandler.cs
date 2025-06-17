using MetinClientless.Libs;
using MetinClientless.Packets;
using MetinClientless.Packets.Send;

namespace MetinClientless.Handlers;

public class PhaseChangeHandler : IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var packet = PacketGCPhase.Read(data);
        Console.WriteLine($"Phase changed to {packet.Phase}");
        
        GameState.CurrentPhase = packet.Phase;
        
        if (packet.Phase == EPhase.PHASE_AUTH)
        {
            // Cipher.Dispose();HEADER_CG_LOGIN4
            // await SocketHandler.GetInstance(null).ChangePortAsync(13001);
            
            var bytes = PacketCGLogin4.Serialize();
            bytes = bytes.Concat(PacketCGLogin3.Serialize(Configuration.Account.Username, Configuration.Account.Password, Configuration.Account.Pin)).ToArray();
            
            return bytes;
        }
        if (packet.Phase == EPhase.PHASE_LOGIN)
        {
            return PacketCGLogin2.Serialize(Configuration.Account.Username);
        }
        if (packet.Phase == EPhase.PHASE_LOADING)
        {
            return new BufferBuilder(1, true)
                .AddByte(0x0A)
                .Build();
        }

        return null;
    }
}