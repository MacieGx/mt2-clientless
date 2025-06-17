using MetinClientless.Libs;

namespace MetinClientless.Handlers;

public class KeyAgreementCompletedHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        Cipher.getInstance().isStarted = true;
        return null;
    }
}