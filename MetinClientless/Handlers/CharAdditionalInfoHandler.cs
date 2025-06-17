using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class CharAdditionalInfoHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        PacketGCCharAdditionalInfo charAdditionalInfo = PacketGCCharAdditionalInfo.Read(data);
        GameState.PlayerNameMap[charAdditionalInfo.Id] = charAdditionalInfo.Name;
        return null;
    }
}