using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class AuthSuccessHandler : IPacketHandler
{ 
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var packet = PacketGCAuthSuccess.Read(data);
        GameState.LoginKey = packet.LoginKey;

        GameState.CurrentPhase = EPhase.PHASE_HANDSHAKE;
        
        return null;
    }
    
    public int GetNewPort()
    {
        return Configuration.GameServer.SelectPort;
    }
}