using MetinClientless.Libs;
using MetinClientless.Packets;
using MetinClientless.Packets.Send;

namespace MetinClientless.Handlers;

public class KeyAgreementHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var packet = PacketGCKeyAgreement.Read(data);
        
        var cipher = Cipher.getInstance();
        cipher.Activate(true, packet.agreedValueLength, packet.publicKey);
        
        return PacketCGKeyAgreement.Serialize(cipher.Key);
    }
}