using System.Buffers.Binary;
using MetinClientless.Handlers;
using MetinClientless.Packets;

namespace MetinClientless.Services;

// TODO: We should use transaction here but i don't give a shit rn
public class BuyRequestHandler(SocketHandler socketHandler)
{
    public async Task Handle()
    {
        
        while (true)
        {
            await Task.Delay(500);

            if (GameState.CurrentPhase != EPhase.PHASE_GAME)
            {
                continue;
            }

            if (GameState.IsTradeWindowOpen)
            {
                continue;
            }
            
            var request = await ShoppingDatabaseService.GetFirstRequestedShopPurchase();
            if (request == null)
            {
                GameState.IsBuyingActionInProgress = false;
                continue;
            }

            GameState.IsBuyingActionInProgress = true;

            var playerFeeDetails = await ShoppingDatabaseService.GetPlayerFeeDetails(request.PlayerName);
            
            // Calculate the provision (fee) based on the fee percentage
            var provision = (long)((request.Price * playerFeeDetails.FeePercentage) / 100);
            var finalProvision = provision > playerFeeDetails.FeeCap ? playerFeeDetails.FeeCap : provision;
            var totalToPay = request.Price + finalProvision;

            
            long playerBalance = await ShoppingDatabaseService.GetPlayerBalance(request.PlayerName);
            if (playerBalance < totalToPay)
            {
                Console.WriteLine($"Player {request.PlayerName} does not have enough money to buy item {request.Id}");
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, "Not enough money", false);
                continue;
            }
            
            await Task.Delay(500);
            
            // Close shop
            await socketHandler.SendAsync([0x77, 0x04, 0x00, 0x1F]);
            await Task.Delay(500);

            byte[] openShopBuffer = [0x77, 0x08, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00];
            BinaryPrimitives.WriteUInt32LittleEndian(openShopBuffer.AsSpan(4), request.ShopId);
            await socketHandler.SendAsync(openShopBuffer);


            var shopOpenErrorMesssage = "Timed out while waiting for shop";
            var isWaitingForShopAlreadyHandled = false;
            PacketGCShopContent? packetGcShopContent = null;
            socketHandler.OnPacket(EServerToClient.HEADER_GC_MT2009_SHOP_DATA, (data) =>
            {
                if (isWaitingForShopAlreadyHandled) return true;
                var packet = PacketGCShopContent.Read(data);
                if (packet.Id != request.ShopId) return false;
                isWaitingForShopAlreadyHandled = true;
                
                packetGcShopContent = packet;
                return true;
            });
            
            socketHandler.OnPacket(EServerToClient.HEADER_GC_CHAT, (data) =>
            {
                if (isWaitingForShopAlreadyHandled) return true;
                var packet = PacketGCChat.Read(data);
                
                if (packet.Type != EChatType.CHAT_TYPE_INFO) return false;
                if (!packet.Message.Contains("obok sklepu")) return false;
                
                Console.WriteLine($"Failed to buy item {request.Id} from player {request.PlayerName}. Reason: Bot is too far from the shop to open it");
                shopOpenErrorMesssage = "Bot jest zbyt daleko sklepu, aby go otworzyć";
                
                GameState.ScanShopIds.Remove(request.ShopId);
                GameState.OpenShopIds.Remove(request.ShopId);

                ShoppingDatabaseService.UpdateShopIds();
                
                isWaitingForShopAlreadyHandled = true;
                return true;
            });
            
            _ = Task.Run(async () => {
                await Task.Delay(5000);
                isWaitingForShopAlreadyHandled = true;
            });
            
            
            while (!isWaitingForShopAlreadyHandled)
            {
                await Task.Delay(100);
            }
            
            await socketHandler.SendAsync([0x77, 0x04, 0x00, 0x21]);
            await socketHandler.SendAsync([0x77, 0x04, 0x00, 0x21]);
            
            if (packetGcShopContent == null)
            {
                Console.WriteLine($"Failed to buy item {request.Id} from player {request.PlayerName}. Reason: {shopOpenErrorMesssage}");
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, shopOpenErrorMesssage, false);
                continue;
            }

            var requestItem = packetGcShopContent?.Items.Find(x => x.ItemTransactionId == request.ItemTransactionId);
            
            if (requestItem == null || requestItem?.ItemTransactionId == 0)
            {
                Console.WriteLine($"Failed to buy item {request.Id} from player {request.PlayerName}. Reason: Item is no longer available");
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, "Item is no longer available", false);
                continue;
            }
            
            var itemPrice = requestItem?.Price;
            
            if (itemPrice != request.Price)
            {
                Console.WriteLine($"Failed to buy item {request.Id} from player {request.PlayerName}. Reason: Item price has changed");
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, "Item price has changed", false);
                continue;
            }
            
            try
            {
                await ShoppingDatabaseService.TakePlayerBalance(request.PlayerName, totalToPay);
                await ShoppingDatabaseService.AddFeesCollectedToBot(finalProvision);
                await ShoppingDatabaseService.AddFeesCollectedToPlayer(request.PlayerName, finalProvision);
            } catch (Exception e)
            {
                Console.WriteLine($"CRITICAL: Failed to take money from player {request.PlayerName}. Reason: {e.Message}");
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, e.Message, true);
                continue;
            }
            
            var buyBuffer = new BufferBuilder(21)
                .AddBytes([0x77, 0x15, 0x00, 0x05])
                .AddUInt32((uint)request.ShopTransactionId)
                .AddUInt32((uint)request.ItemTransactionId)
                .AddByte(0x00)
                .AddUInt64((uint)request.Price)
                .Build();
            
            await socketHandler.SendAsync(buyBuffer);

            var isAlreadyHandled = false;
            var shouldRefund = false;
            var errorMessage = "Timed out while waiting for buy response";
            PacketGCItemSet2? packetGCItemSet2 = null;
            
            // [SEND][L:17][UNKNOWN][124][77 15 00 05 || 7C 0B 00 00 DC C8 CE 01 00 53 25 00 00 00 00 00 00 ]
            
            socketHandler.OnPacket(EServerToClient.HEADER_GC_CHAT, (data) =>
            {
                if (isAlreadyHandled) return true;
                
                var packet = PacketGCChat.Read(data);
                if (packet.Type != EChatType.CHAT_TYPE_INFO) return false;

                if (packet.Message.Contains("Sklep jest w trybie edycji."))
                {
                    shouldRefund = true;
                }
                
                if (packet.Message.Contains("Cena przedmiotu zmieniła się."))
                {
                    shouldRefund = true;
                }

                if (packet.Message.Contains("Sklep został niedawno edytowany przez właściciela. Odczekaj chwilę przed zakupem."))
                {
                    shouldRefund = true;
                }
                
                errorMessage = packet.Message;
                isAlreadyHandled = true;
                return true;
            });
            
            socketHandler.OnPacket(EServerToClient.HEADER_GC_ITEM_SET2, (data) =>
            {
                if (isAlreadyHandled) return true;
                
                packetGCItemSet2 = PacketGCItemSet2.Read(data);
                isAlreadyHandled = true;
                return true;
            });
            
            _ = Task.Run(async () => {
                await Task.Delay(10000);
                isAlreadyHandled = true;
            });
            
            
            while (!isAlreadyHandled)
            {
                await Task.Delay(100);
            }
            
            if (packetGCItemSet2 == null)
            {
                Console.WriteLine($"Failed to buy item {request.Id} from player {request.PlayerName}. Reason: {errorMessage}");
                
                if (shouldRefund)
                {
                    Console.WriteLine($"Refunding player {request.PlayerName} {totalToPay} because of the handled error");
                    await ShoppingDatabaseService.AddPlayerBalance(request.PlayerName, totalToPay);
                    await ShoppingDatabaseService.TakeFeesCollectedFromBot(finalProvision);
                    await ShoppingDatabaseService.TakeFeesCollectedFromPlayer(request.PlayerName, finalProvision);
                }
                
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, errorMessage, !shouldRefund);
                continue;
            }
            
            Console.WriteLine($"Successfully bought item {request.Id} from player {request.PlayerName}");

            try
            {
                await ShoppingDatabaseService.AddPlayerItem(request.PlayerName, request.ItemId, (PacketGCItemSet2)packetGCItemSet2);
                await ShoppingDatabaseService.SetShopPurchaseStatusToCompleted(request.Id, finalProvision);
            } catch (Exception e)
            {
                Console.WriteLine($"CRITICAL: Failed to add item to player {request.PlayerName}. Reason: {e.Message}");
                await ShoppingDatabaseService.SetShopPurchaseStatusToFailed(request.Id, e.Message, true);
            }
            
        }
    }
}