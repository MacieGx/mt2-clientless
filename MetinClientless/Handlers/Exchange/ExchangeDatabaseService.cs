using MetinClientless.Services;

namespace MetinClientless.Handlers;

public class ExchangeDatabaseService
{
    public static async Task<bool> StartMoneyDeposit(ExchangeState exchangeState)
    {
        if (exchangeState.Action != TradeAction.DEPOSIT)
        {
            throw new Exception("Trying to start money deposit with wrong action");
        }
        
        var sql = @"
        INSERT INTO money_transactions (
            bot_id,
            player_name,
            transaction_type,
            amount
        ) VALUES (
            @BotId,
            @PlayerName,
            'DEPOSIT',
            @Amount
        ) RETURNING id;";

        var parameters = new Dictionary<string, object>
        {
            { "@BotId", GameState.BotId },
            { "@PlayerName", exchangeState.GetPlayerName() },
            { "@Amount", Convert.ToInt64(exchangeState.GoldFromPlayer) }
        };

        try
        {
            var id = await DatabaseService.ExecuteQueryReturningAsync<Guid>(sql, parameters);

            if (id == null) return false;


            exchangeState.MoneyTransactionId = id.Value;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while starting money deposit: {e.Message}");
            return false;
        }
    }

    public static async Task<bool> FinishMoneyDeposit(ExchangeState exchangeState)
    {
        if (exchangeState.Action != TradeAction.DEPOSIT)
        {
            throw new Exception("Trying to finish money deposit with wrong action");
        }
        
        var transactionStatus = exchangeState.Accepted ? "FAILED" : "CANCELED";

        string errorMessage = exchangeState.Accepted ? null : "Deposit was not accepted";

        try
        {
            if (exchangeState.Accepted)
            {
                var playerBalanceSql = @"
            INSERT INTO player_balance (
                bot_id,
                player_name,
                balance
            ) VALUES (
                @BotId::uuid,
                @CharacterName,
                @MoneyDeposit
            )
            ON CONFLICT (bot_id, player_name) 
            DO UPDATE SET
                balance = player_balance.balance + @MoneyDeposit,
                updated_at = CURRENT_TIMESTAMP
            RETURNING id;";

                var playerBalanceParameters = new Dictionary<string, object>
                {
                    { "@BotId", GameState.BotId },
                    { "@CharacterName", exchangeState.GetPlayerName() },
                    { "@MoneyDeposit", Convert.ToInt64(exchangeState.GoldFromPlayer) }
                };

                await DatabaseService.ExecuteQueryAsync(playerBalanceSql, playerBalanceParameters);
                transactionStatus = "COMPLETED";
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while finishing money deposit: {e.Message}");
            errorMessage = e.Message;
            return false;
        }
        finally
        {
            var sql = @"
            UPDATE money_transactions
            SET transaction_status = @TransactionStatus::transaction_status,
                error_message = @ErrorMessage
            WHERE id = @TransactionId::uuid;";

            var parameters = new Dictionary<string, object>
            {
                { "@TransactionId", exchangeState.MoneyTransactionId },
                { "@TransactionStatus", transactionStatus },
                { "@ErrorMessage", (object)(errorMessage ?? null) ?? DBNull.Value }
            };

            await DatabaseService.ExecuteQueryAsync(sql, parameters);
        }
    }
    
    public static async Task<ulong> StartWithdrawReturningBalance(ExchangeState exchangeState)
    {
        if (exchangeState.Action != TradeAction.WITHDRAW)
        {
            throw new Exception("Trying to start money withdraw with wrong action");
        }
    
        try
        {
            var sql = @"
            WITH current_balance AS (
                SELECT balance 
                FROM player_balance 
                WHERE bot_id = @BotId::uuid AND player_name = @PlayerName
                AND balance > 0
                LIMIT 1
            ),
            new_transaction AS (
                INSERT INTO money_transactions (
                    bot_id,
                    player_name,
                    transaction_type,
                    amount
                ) 
                SELECT 
                    @BotId::uuid,
                    @PlayerName,
                    'WITHDRAW'::transaction_type,
                    balance
                FROM current_balance
                RETURNING id, amount
            )
            SELECT id, amount FROM new_transaction
            UNION ALL
            SELECT NULL::uuid, 0
            LIMIT 1;";

            var parameters = new Dictionary<string, object>
            {
                { "@BotId", GameState.BotId },
                { "@PlayerName", exchangeState.GetPlayerName() },
            };

            var result = await DatabaseService.ExecuteQueryReturningTupleAsync<Guid, long>(sql, parameters);
        
            if (result.Item1 == null || result.Item2 == null)
            {
                return 0;
            }
        
            exchangeState.MoneyTransactionId = result.Item1.Value;

            return (ulong)result.Item2;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while starting money withdraw: {e.Message}");
            return 0;
        }
    }
    
    
    public static async Task<bool> FinishWithdrawBalance(ExchangeState exchangeState)
    {
        if (exchangeState.Action != TradeAction.WITHDRAW)
        {
            throw new Exception("Trying to start money withdraw with wrong action");
        }
        
        
        var transactionStatus = exchangeState.Accepted ? "FAILED" : "CANCELED";
        var errorMessage = exchangeState.Accepted ? null : "Withdraw was not accepted";

        try
        {
            if (exchangeState.Accepted)
            {
                var playerBalanceSql = @"
            UPDATE player_balance 
            SET 
                balance = 0,
                updated_at = CURRENT_TIMESTAMP
            WHERE 
                bot_id = @BotId::uuid AND 
                player_name = @PlayerName 
            RETURNING id;";

                var playerBalanceParameters = new Dictionary<string, object>
                {
                    { "@BotId", GameState.BotId },
                    { "@PlayerName", exchangeState.GetPlayerName() }
                };

                await DatabaseService.ExecuteQueryAsync(playerBalanceSql, playerBalanceParameters);
                await ShoppingDatabaseService.AddFeesCollectedToBot(1);
                await ShoppingDatabaseService.AddFeesCollectedToPlayer(exchangeState.GetPlayerName(), 1);
                transactionStatus = "COMPLETED";
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while finishing withdraw: {e.Message}");
            errorMessage = e.Message;
            return false;
        }
        finally
        {
            var sql = @"
        UPDATE money_transactions
        SET 
            transaction_status = @TransactionStatus::transaction_status,
            error_message = @ErrorMessage
        WHERE id = @TransactionId::uuid;";

            var parameters = new Dictionary<string, object>
            {
                { "@TransactionId", exchangeState.MoneyTransactionId },
                { "@TransactionStatus", transactionStatus },
                { "@ErrorMessage", (object)(errorMessage ?? null) ?? DBNull.Value }
            };

            await DatabaseService.ExecuteQueryAsync(sql, parameters);
        }
    }
    
    public static async Task<List<PlayerItem>> GetAvailablePlayerItems(string playerName)
{
    var sql = @"
        SELECT 
            id as Id,
            bot_id as BotId,
            player_name as PlayerName,
            item_id as ItemId,
            count as Count,
            inventory_position as InventoryPosition,
            withdrawn as Withdrawn,
            created_at as CreatedAt,
            updated_at as UpdatedAt
        FROM player_items 
        WHERE 
            player_name = @PlayerName 
            AND withdrawn = false
            AND bot_id = @BotId::uuid;";

    var parameters = new Dictionary<string, object>
    {
        { "@PlayerName", playerName },
        { "@BotId", GameState.BotId }
    };

    var items = new List<PlayerItem>();
    await using var command = DatabaseService._dataSource.CreateCommand(sql);
    if (parameters != null)
    {
        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Key, param.Value);
        }
    }

    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var item = new PlayerItem
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            BotId = reader.GetGuid(reader.GetOrdinal("BotId")),
            PlayerName = reader.GetString(reader.GetOrdinal("PlayerName")),
            ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
            Count = reader.GetInt32(reader.GetOrdinal("Count")),
            InventoryPosition = reader.GetInt32(reader.GetOrdinal("InventoryPosition")),
            Withdrawn = reader.GetBoolean(reader.GetOrdinal("Withdrawn")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
        items.Add(item);
    }

    return items;
}

public static async Task SetItemsWithdrawn(List<Guid> itemIds)
{
    var sql = @"
        UPDATE player_items 
        SET 
            withdrawn = true,
            updated_at = CURRENT_TIMESTAMP
        WHERE id = ANY(@ItemIds::uuid[]);";

    var parameters = new Dictionary<string, object>
    {
        { "@ItemIds", itemIds }
    };

    await DatabaseService.ExecuteQueryAsync(sql, parameters);
}
    
    
}