using System.Buffers.Binary;
using MetinClientless.Packets;
using MetinClientless.Services;

namespace MetinClientless.Handlers;

public class ShopDataHandler : IPacketHandler
{
    // SHOP ON OPEN:
    // D6 4A 00 00 00 13 32 0C 00 00 45 53 53 41 20 53 4B 4C 45 50 49 4B 20 42 59 43 5A 4B 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 69 65 6C 6E 69 63 7A 6B 61 00 00 00 00 00 00 00 00 00 C3 39 07 00 98 94 0E 00 00 00 00 00
    
    // SHOP ON CLOSE:
    // D6 0A 00 00 00 14 32 0C 00 00
    
    // SHOP ID: 32 0C 00 00
    // SHOP NAME: 45 53 53 41 20 53 4B 4C 45 50 49 4B 20 42 59 43 5A 4B 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    // CHARACTER NAME:  5A 69 65 6C 6E 69 63 7A 6B 61 00 00 00 00 00 00 00 00 00
    // COORDINATES I GUESS: C3 39 07 00 98 94 0E 00 00 00 00 00 but idk how to decode
    
    
    // 0A and D6 is Lenght cus its dynamic packet
    
    public static long lastTimeShopsIdsUpdated = 0;

    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        
        long currentMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentMs - lastTimeShopsIdsUpdated > 2500)
        {
            _ = ShoppingDatabaseService.UpdateShopIds();
            lastTimeShopsIdsUpdated = currentMs;
        }
        
        if (data.Length > 100)
        {
            Console.WriteLine("Received shop content packet");
            var shop = PacketGCShopContent.Read(data);
            
            GameState.ScanShopIds.Remove(shop.Id);
            GameState.ScanShopIds.Add(shop.Id);

            try
            {
                var query = SqlQueryBuilder.BuildInsertQuery(shop);
                _ = DatabaseService.ExecuteQueryAsync(query);
            } catch (Exception e)
            {
                Console.WriteLine(e);
                
                Console.WriteLine($"Shop ID: {shop.Id} Name: {shop.Name} PlayerName: {shop.PlayerName}");
                foreach (var item in shop.Items)
                {
                    Console.WriteLine($"Item: {item.Vnum} Pos: {item.Pos} Count: {item.Count} Price: {item.Price}");
                }
                
                Environment.Exit(1);
            }

            _ = UpdateLastShopReceivedAt();
            
            return null;
        }

        var packet = PacketGCShopData.Read(data);
        Console.WriteLine($"Shop ({packet.Id}) is {packet.State}");

        switch (packet.State)
        {
            case ShopState.OPEN:
            {
                if (!GameState.OpenShopIds.Contains(packet.Id))
                {
                    GameState.OpenShopIds.Add(packet.Id);
                }
                
                if (packet.Id % Configuration.Account.ShopIdModulo != Configuration.Account.ShopIdExpectedModuloValue)
                    break;
                
                if (GameState.ScanShopIds.Contains(packet.Id))
                    break;

                GameState.ScanShopIds.Add(packet.Id);
                
                
                Console.WriteLine($"Having {GameState.ScanShopIds.Count} shops open");
                
                break;
            }
            case ShopState.CLOSED:
            {
                GameState.ScanShopIds.Remove(packet.Id);
                GameState.OpenShopIds.Remove(packet.Id);
                break;
            }
        }

        return null;
    }
    
    
    private async Task<bool> UpdateLastShopReceivedAt()
    {
        var statusQuery = $"UPDATE bots SET last_shop_received_at = CURRENT_TIMESTAMP WHERE id = @BotId::uuid;";
        var parameters = new Dictionary<string, object>
        {
            { "@BotId", GameState.BotId }
        };
        
        
        await DatabaseService.ExecuteQueryAsync(statusQuery, parameters);
        return true;
    }

}