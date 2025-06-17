using System.Buffers.Binary;
using MetinClientless.Packets;

namespace MetinClientless.Services;

public class ShopHandler(SocketHandler socketHandler)
{
    public async Task Handle()
    {
        long shopRefreshStartedAt = 0;
        int lastShopId = 0;
        
        while (true)
        {
            await Task.Delay(550);
            if (GameState.ShopScanningPaused || GameState.IsBuyingActionInProgress || GameState.IsTradeWindowOpen)
            {
                long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                shopRefreshStartedAt = now;
                
                
                if (GameState.ShopScanningPaused && GameState.ShopScanningPausedEndTimestampMs != 0 && GameState.ShopScanningPausedEndTimestampMs < now)
                {
                    GameState.ShopScanningPaused = false;
                    GameState.ShopScanningPausedEndTimestampMs = 0;
                }
                
                continue;
            }
            if (GameState.ScanShopIds.Count == 0) continue;
            
            uint shopId = GameState.ScanShopIds[0];
            
            if (lastShopId == shopId && GameState.ScanShopIds.Count > 1)
            {
                if (shopRefreshStartedAt + 5000 < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds())
                {
                    GameState.ScanShopIds.Remove(shopId);
                    GameState.ScanShopIds.Remove(shopId);
                    continue;   
                }
            }
            else
            {
                lastShopId = (int)shopId;
                shopRefreshStartedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            }
            
            byte[] buffer = [0x77, 0x04, 0x00, 0x21, 0x77, 0x04, 0x00, 0x1F, 0x77, 0x08, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00];
            BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(12), shopId);


            socketHandler.OnPacket(EServerToClient.HEADER_GC_CHAT, (data) =>
            {
                if (shopId != lastShopId) return true;
                var packet = PacketGCChat.Read(data);
                
                if (packet.Type != EChatType.CHAT_TYPE_INFO) return false;
                if (!packet.Message.Contains("obok sklepu")) return false;
                
                GameState.ScanShopIds.Remove(shopId);
                GameState.OpenShopIds.Remove(shopId);
                return true;
            });

            await socketHandler.SendAsync(buffer);
            GameState.IsWaitingForShopRecv = true;
        }
    }
}