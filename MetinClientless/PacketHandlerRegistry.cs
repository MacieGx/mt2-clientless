using MetinClientless.Handlers;
using MetinClientless.Packets;

namespace MetinClientless;

public class PacketHandlerRegistry
{
    private readonly Dictionary<int, IPacketHandler> _handlers = new Dictionary<int, IPacketHandler>();

    public void RegisterHandler(EServerToClient header, IPacketHandler handler)
    {
        _handlers[(int)header] = handler;
    }

    public async Task<PacketResponse> ProcessPacketAsync(int header, byte[] data)
    {
        if (_handlers.TryGetValue(header, out var handler))
        {
            var packetData = await handler.HandlePacketAsync(data);
            var newPort = handler.GetNewPort();
            
            return new PacketResponse() { Data = packetData, NewPort = newPort };
        }
        
        return new PacketResponse() { Data = [], NewPort = 0 };
    }
}