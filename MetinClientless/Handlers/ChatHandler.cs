using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class ChatHandler: IPacketHandler
{
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var chat = PacketGCChat.Read(data);
        
        if (chat.Type is EChatType.CHAT_TYPE_INFO or EChatType.CHAT_TYPE_NOTICE or EChatType.CHAT_TYPE_COMMAND or EChatType.CHAT_TYPE_WHISPER)
        {
            Console.WriteLine($"[INFO][{chat.Type}] {chat.Message}");
        }
        
        
        if (ExchangeHandler.ExchangeState.GetIsStarted())
        {
            if (chat.Type == EChatType.CHAT_TYPE_INFO && chat.Message.Equals($"Handel z {ExchangeHandler.ExchangeState.GetPlayerName()} zakończony pomyślnie."))
            {
                ExchangeHandler.ExchangeState.Accepted = true;
            }
        }
        
        if (chat.Type == EChatType.CHAT_TYPE_COMMAND && chat.Message.StartsWith("messenger_auth"))
        {
            var playerName = chat.Message.Split(' ')[1];
            
            if (!Configuration.TradeAllowedCharacters.Contains(playerName))
            {
                return null;
            }
            
            Console.WriteLine($"[INFO] Received friend request from trade allowed character: {playerName}");
            
            if (!GameState.ShopScanningPaused && !GameState.IsBuyingActionInProgress)
            {
                GameState.ShopScanningPausedEndTimestampMs = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 5000;
                GameState.ShopScanningPaused = true;

                return [0x77, 0x04, 0x00, 0x21, 0x77, 0x04, 0x00, 0x1F];
            }
        }

        return null;
    }
}