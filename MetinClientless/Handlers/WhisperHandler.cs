using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class WhisperHandler : IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var whisper = PacketGCWhisper.Read(data);
        
        if (!Configuration.TradeAllowedCharacters.Contains(whisper.NameFrom))
        {
            return null;
        }
        Console.WriteLine($"[INFO] Received whisper from trade allowed character: {whisper.NameFrom}");
            
        if (!GameState.ShopScanningPaused && !GameState.IsBuyingActionInProgress)
        {
            GameState.ShopScanningPausedEndTimestampMs = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 5000;
            GameState.ShopScanningPaused = true;

            return [0x77, 0x04, 0x00, 0x21, 0x77, 0x04, 0x00, 0x1F];
        }

        return null;
    }
}