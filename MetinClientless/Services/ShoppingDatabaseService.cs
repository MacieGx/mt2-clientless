using MetinClientless.Packets;

namespace MetinClientless.Services;

public class ShopPurchaseRequest
{
    public Guid Id { get; set; }
    public uint ItemId { get; set; }
    public uint ShopId { get; set; }
    public string PlayerName { get; set; }
    public uint ShopTransactionId { get; set; }
    public uint ItemTransactionId { get; set; }
    public long Price { get; set; }
}

public class PlayerFeeDetails
{
    public decimal FeePercentage { get; set; }
    public long FeeCap { get; set; }
}

public class ShoppingDatabaseService
{
    public static async Task<ShopPurchaseRequest?> GetFirstRequestedShopPurchase()
    {
        try
        {
            var sql = @"
            WITH first_request AS (
                SELECT 
                    id,
                    player_name,
                    shop_id,
                    item_id
                    shop_transaction_id,
                    item_transaction_id,
                    price
                FROM shop_purchases
                WHERE 
                    bot_id = @BotId::uuid 
                    AND transaction_status = 'REQUESTED'::transaction_status
                LIMIT 1
                FOR UPDATE
            )
            UPDATE shop_purchases
            SET transaction_status = 'PENDING'::transaction_status
            WHERE id = (SELECT id FROM first_request)
            RETURNING 
                id as Id,
                player_name as PlayerName,
                shop_id as ShopId,
                item_id as ItemId,
                shop_transaction_id as ShopTransactionId,
                item_transaction_id as ItemTransactionId,
                price as Price;";

            var parameters = new Dictionary<string, object>
            {
                { "@BotId", GameState.BotId }
            };

            return await DatabaseService.ExecuteQueryReturningObjectAsync<ShopPurchaseRequest>(sql, parameters);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while getting pending shop purchase: {e.Message}");
            return null;
        }
    }
    
    public static async Task<long> GetPlayerBalance(string playerName)
    {
        try
        {
            var sql = @"
           SELECT COALESCE(balance, 0) as balance
           FROM player_balance
           WHERE bot_id = @BotId::uuid 
           AND player_name = @PlayerName;";

            var parameters = new Dictionary<string, object>
            {
                { "@BotId", GameState.BotId },
                { "@PlayerName", playerName }
            };

            return await DatabaseService.ExecuteQueryReturningAsync<long>(sql, parameters) ?? 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while getting player balance: {e.Message}");
            return 0;
        }
    }
    
    public static async Task<PlayerFeeDetails> GetPlayerFeeDetails(string playerName)
    {
        try
        {
            var sql = @"
           SELECT 
               fee_percentage as FeePercentage,
               fee_cap as FeeCap
           FROM players
           WHERE nickname = @PlayerName;";

            var parameters = new Dictionary<string, object>
            {
                { "@PlayerName", playerName }
            };

            // Execute the query and return a tuple (fee_percentage, fee_cap)
            var result = await DatabaseService.ExecuteQueryReturningObjectAsync<PlayerFeeDetails>(sql, parameters);

            if (result == null)
            {
                Console.WriteLine("Player not found or no fee details available.");
                return new PlayerFeeDetails { FeePercentage = 0m, FeeCap = 0 }; // Default values
            }

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while getting player fee details: {e.Message}");
            return new PlayerFeeDetails { FeePercentage = 0m, FeeCap = 0 };
        }
    }

    public static async Task<bool> SetShopPurchaseStatusToFailed(Guid purchaseId, string reason, bool moneyTaken)
    {
        try
        {
            var sql = @"
           UPDATE shop_purchases
           SET 
               transaction_status = 'FAILED'::transaction_status,
               error_message = @Reason,
               money_taken = @MoneyTaken
           WHERE id = @PurchaseId::uuid;";

            var parameters = new Dictionary<string, object>
            {
                { "@PurchaseId", purchaseId },
                { "@Reason", reason },
                { "@MoneyTaken", moneyTaken }
            };

            await DatabaseService.ExecuteQueryAsync(sql, parameters);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while setting shop purchase status to failed: {e.Message}");
            return false;
        }
    }
    
    public static async Task<bool> SetShopPurchaseStatusToCompleted(Guid purchaseId, long feeAmount)
    {
        try
        {
            var sql = @"
           UPDATE shop_purchases
           SET 
               transaction_status = 'COMPLETED'::transaction_status,
               error_message = null,
               fees_collected = @FeeAmount,
               money_taken = true
           WHERE id = @PurchaseId::uuid;";

            var parameters = new Dictionary<string, object>
            {
                { "@PurchaseId", purchaseId },
                { "@FeeAmount", feeAmount }
            };

            await DatabaseService.ExecuteQueryAsync(sql, parameters);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while setting shop purchase status to failed: {e.Message}");
            return false;
        }
    }
    
    public static async Task<long> AddPlayerBalance(string playerName, long amount)
    {
        try
        {
            var sql = @"
           UPDATE player_balance
           SET balance = balance + @Amount
           WHERE bot_id = @BotId::uuid 
           AND player_name = @PlayerName
           RETURNING balance;";

            var parameters = new Dictionary<string, object>
            {
                { "@BotId", GameState.BotId },
                { "@PlayerName", playerName },
                { "@Amount", (int)amount }
            };

            return await DatabaseService.ExecuteQueryReturningAsync<long>(sql, parameters) ?? 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while adding player balance: {e.Message}");
            return 0;
        }
    }
    
    public static async Task<long> TakePlayerBalance(string playerName, long amount)
    {
        return await AddPlayerBalance(playerName, -amount);
    }
    
    public static async Task<Guid?> AddPlayerItem(string playerName, uint itemId, PacketGCItemSet2 packetItemSet)
    {
        var sql = @"
           INSERT INTO player_items (
               bot_id,
               player_name,
               item_id,
               count,
               inventory_position
           ) VALUES (
               @BotId::uuid,
               @PlayerName,
               @ItemId,
               @Count,
               @InventoryPosition
           )
           RETURNING id;";

        var parameters = new Dictionary<string, object>
        {
            { "@BotId", GameState.BotId },
            { "@PlayerName", playerName },
            { "@ItemId", (int)itemId },
            { "@Count", (int)packetItemSet.Count },
            { "@InventoryPosition", (int)packetItemSet.InventoryPos.Cell }
        };

        return await DatabaseService.ExecuteQueryReturningAsync<Guid>(sql, parameters);
    }
    
    public static async Task<bool> AddFeesCollectedToBot(long feeAmount)
    {
        try
        {
            var sql = @"
            UPDATE bots
            SET pending_fees = pending_fees + @FeeAmount
            WHERE id = @BotId::uuid;";

            var parameters = new Dictionary<string, object>
            {
                { "@BotId", GameState.BotId },
                { "@FeeAmount", feeAmount }
            };

            await DatabaseService.ExecuteQueryAsync(sql, parameters);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while adding fees to bot: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> TakeFeesCollectedFromBot(long feeAmount)
    {
        return await AddFeesCollectedToBot(-feeAmount);
    }

    public static async Task<bool> AddFeesCollectedToPlayer(string playerName, long feeAmount)
    {
        try
        {
            var sql = @"
            UPDATE players
            SET total_fees_collected = total_fees_collected + @FeeAmount
            WHERE nickname = @PlayerName;";

            var parameters = new Dictionary<string, object>
            {
                { "@PlayerName", playerName },
                { "@FeeAmount", feeAmount }
            };

            await DatabaseService.ExecuteQueryAsync(sql, parameters);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while adding fees to player: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> TakeFeesCollectedFromPlayer(string playerName, long feeAmount)
    {
        return await AddFeesCollectedToPlayer(playerName, -feeAmount);
    }
    
    public static async Task<bool> UpdateShopIds()
    {
        var statusQuery = $"UPDATE bots SET available_shops = @ShopIds::integer[] WHERE id = @BotId::uuid;";
        var parameters = new Dictionary<string, object>
        {
            { "@BotId", GameState.BotId },
            // Convert uint values to int values before sending to PostgreSQL
            { "@ShopIds", GameState.OpenShopIds.Select(x => (int)x).ToArray() }
        };
    
        await DatabaseService.ExecuteQueryAsync(statusQuery, parameters);
        return true;
    }
}
