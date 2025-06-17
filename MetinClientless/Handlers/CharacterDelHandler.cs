using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class CharacterDelHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var packet = PacketGCCharacterDel.Read(data);
        GameState.PlayerNameMap.Remove(packet.Id);
        Console.WriteLine($"Character deleted: {packet.Id}");
        return null;
    }
}