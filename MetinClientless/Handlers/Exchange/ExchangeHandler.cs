using MetinClientless.Packets;

namespace MetinClientless.Handlers;

public class ExchangeHandler: IPacketHandler
{
    public static ExchangeState ExchangeState = new();
    
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        var exchange = PacketGCExchange.Read(data);

        if (exchange.SubHeader == ExchangeSubHeaderGC.EXCHANGE_SUBHEADER_GC_START)
        {
            GameState.IsTradeWindowOpen = true;
            if (ExchangeState.GetIsStarted())
            {
                Console.WriteLine("ERROR: Exchange already started");
                ExchangeState.Reset();
                return PacketCGExchange.Cancel();
            }
            
            var playerExists = GameState.PlayerNameMap.TryGetValue((uint)exchange.arg1, out var playerName);
            
            if (!playerExists || playerName == null) {
                Console.WriteLine("ERROR: Player does not exist in the player name map at EXCHANGE_SUBHEADER_GC_START!");
                return null;
            }
            
            var isPlayerOnAllowList = Configuration.TradeAllowedCharacters.Contains(playerName);
            
            if (!isPlayerOnAllowList)
            {
                Console.WriteLine($"Player {playerName} is not on the allow list");
                return null;
            }
            
            var playerItems = await ExchangeDatabaseService.GetAvailablePlayerItems(playerName);
            
            ExchangeState.Start(playerName, playerItems.Count > 0);
            Console.WriteLine($"Exchange started with {playerName}");
            
            if (!playerItems.Any())
            {
                return null;
            }
            
            var optimizedPlacement = new TradeWindowHelper().OptimizePlacement(playerItems);
            
            if (optimizedPlacement.Count == 0)
            {
                Console.WriteLine("ERROR: No optimized placement found");
                ExchangeState.Reset();
                return PacketCGExchange.Cancel();
            }
            
            ExchangeState.WithdrawItemGuids = optimizedPlacement.Select(p => p.ItemGuid).ToList();
            
            Console.WriteLine($"Optimized placement found for {optimizedPlacement.Count} items");


            try
            {
                foreach (var i in optimizedPlacement)
                {
                    await SocketHandler.GetInstance(null)
                        .SendAsync(PacketCGExchange.AddItem((ushort)i.InventoryPosition, (byte)i.CellIndex));
                    await Task.Delay(150);
                }                                      
            } catch (Exception e)
            {
                Console.WriteLine($"Error while adding items to the exchange: {e.Message}");
                ExchangeState.Reset();
                return PacketCGExchange.Cancel();
            }
 
            return null;
        }
        
        if (exchange.SubHeader == ExchangeSubHeaderGC.EXCHANGE_SUBHEADER_GC_END)
        {
            GameState.IsTradeWindowOpen = false;
            
            if (!ExchangeState.GetIsStarted())
            {
                return null;
            }
            
            if (ExchangeState.Accepted)
            {
                Console.WriteLine($"Exchange with {ExchangeState.GetPlayerName()} was successful");
            }
            else
            {
                Console.WriteLine($"Exchange with {ExchangeState.GetPlayerName()} was not successful");
            }
            
            
            if (!ExchangeState.MoneyTransactionId.Equals(Guid.Empty))
            {
                if (ExchangeState.Action == TradeAction.DEPOSIT)
                {
                    var success = await ExchangeDatabaseService.FinishMoneyDeposit(ExchangeState);
                    
                    if (!success)
                    {
                        Console.WriteLine("Error while finishing money deposit");
                    }
                }
                
                if (ExchangeState.Action == TradeAction.WITHDRAW)
                {
                    var success = await ExchangeDatabaseService.FinishWithdrawBalance(ExchangeState);
                    
                    if (!success)
                    {
                        Console.WriteLine("Error while finishing money withdraw");
                    }
                }
            }
            
            if (ExchangeState.Action == TradeAction.COLLECT_ITEMS && ExchangeState.WithdrawItemGuids.Count != 0 && ExchangeState.Accepted)
            {
                await ExchangeDatabaseService.SetItemsWithdrawn(ExchangeState.WithdrawItemGuids);
                Console.WriteLine("Items withdrawn successfully");
            }
            
            
            ExchangeState.Reset();
            return null;
        }

        if (exchange.SubHeader == ExchangeSubHeaderGC.EXCHANGE_SUBHEADER_GC_ELK_ADD)
        {
            if (!ExchangeState.GetIsStarted())
            {
                return null;
            }
            
            if (exchange.IsMe)
            {
                return null;
            }
            
            if (ExchangeState.Action == TradeAction.COLLECT_ITEMS)
            {
                Console.WriteLine("Player added gold to the exchange, but we are waiting for items");
                ExchangeState.Reset();
                return PacketCGExchange.Cancel();
            }
            
            // TODO: If player will add only 1 gold to the exchange, its key to transfer back all gold belongs to player
            if (exchange.arg1 == 1)
            {
                ExchangeState.Action = TradeAction.WITHDRAW;
                var playerBalance = await ExchangeDatabaseService.StartWithdrawReturningBalance(ExchangeState);
                
                Console.WriteLine($"Player added 1 gold to the exchange, returning {(uint)playerBalance} gold");

                if (playerBalance == 0)
                {
                    ExchangeState.Reset();
                    return PacketCGExchange.Cancel();
                }
                
                return PacketCGExchange.AddGold((uint)playerBalance).Concat(PacketCGExchange.Accept()).ToArray();
            }

            ExchangeState.Action = TradeAction.DEPOSIT;
            ExchangeState.GoldFromPlayer += exchange.arg1;
            Console.WriteLine($"Player added {exchange.arg1} gold to the exchange");
            return null;
        }
        
        
        if (exchange.SubHeader == ExchangeSubHeaderGC.EXCHANGE_SUBHEADER_GC_LESS_ELK)
        {
            if (!ExchangeState.GetIsStarted())
            {
                return null;
            }
            
            
            ExchangeState.Reset();
            return PacketCGExchange.Cancel();
        }

        if (exchange.SubHeader == ExchangeSubHeaderGC.EXCHANGE_SUBHEADER_GC_ACCEPT)
        {
            if (!ExchangeState.GetIsStarted())
            {
                return null;
            }
            
            if (exchange.IsMe)
            {
                return null;
            }
            
            if (ExchangeState.Action == TradeAction.COLLECT_ITEMS)
            {
                if (ExchangeState.RecvItemsPlacedCount != ExchangeState.WithdrawItemGuids.Count)
                {
                    Console.WriteLine("Player accepted the exchange, but we are waiting for items");
                    ExchangeState.Reset();
                    return PacketCGExchange.Cancel();
                }
                
                return PacketCGExchange.Accept();
            }
            
            if (ExchangeState.GoldFromPlayer != 0)
            {
                var success = await ExchangeDatabaseService.StartMoneyDeposit(ExchangeState);
            
                if (!success)
                {
                    Console.WriteLine("Error while starting money deposit");
                    ExchangeState.Reset();
                    return PacketCGExchange.Cancel();
                }
            }
            
            return PacketCGExchange.Accept();
        }
        
        if (exchange.SubHeader == ExchangeSubHeaderGC.EXCHANGE_SUBHEADER_GC_ITEM_ADD)
        {
            if (!ExchangeState.GetIsStarted())
            {
                return null;
            }
            
            if (!exchange.IsMe)
            {
                ExchangeState.Reset();
                return PacketCGExchange.Cancel();
            }
            
            if (ExchangeState.Action != TradeAction.COLLECT_ITEMS)
            {
                Console.WriteLine("Player added item to the exchange, but we are waiting for gold");
                ExchangeState.Reset();
                return PacketCGExchange.Cancel();
            }
            
            ExchangeState.RecvItemsPlacedCount++;

            
            if (ExchangeState.RecvItemsPlacedCount == ExchangeState.WithdrawItemGuids.Count)
            {
                return PacketCGExchange.Accept();
            }
            return null;
        }

        
        return null;
    }
}

public class ExchangeState
{
    public TradeAction Action;
    private bool _IsStarted;
    private string _PlayerName;
    public Guid MoneyTransactionId = Guid.Empty;
    public ulong GoldFromPlayer;
    public bool Accepted;
    public List<Guid> WithdrawItemGuids = new();
    public int RecvItemsPlacedCount = 0;
    
    public bool GetIsStarted()
    {
        return _IsStarted;
    }
    
    public string GetPlayerName()
    {
        return _PlayerName;
    }
    
    public void Reset()
    {
        Action = TradeAction.UNKNOWN;
        _IsStarted = false;
        _PlayerName = null;
        MoneyTransactionId = Guid.Empty;
        GoldFromPlayer = 0;
        WithdrawItemGuids = new List<Guid>();
        RecvItemsPlacedCount = 0;
        Accepted = false;
    }
    
    public void Start(string playerName, bool hasItemsToReceive)
    {
        _IsStarted = true;
        _PlayerName = playerName;

        if (hasItemsToReceive)
        {
            Action = TradeAction.COLLECT_ITEMS;
        }
    }
}

public enum TradeAction
{
    UNKNOWN,
    DEPOSIT,
    WITHDRAW,
    COLLECT_ITEMS
}