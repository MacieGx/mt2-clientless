using MetinClientless.Handlers;

namespace MetinClientless.Packets;

public class LoginFailureHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        Console.WriteLine("Login failed.");
        Environment.Exit(1);
        return null;
    }
}