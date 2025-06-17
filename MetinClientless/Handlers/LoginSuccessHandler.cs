using MetinClientless.Packets;
using Console = System.Console;

namespace MetinClientless.Handlers;

public class LoginSuccessHandler : IPacketHandler
{
    public static bool alreadySwitched = false;
    
    public async Task<byte[]> HandlePacketAsync(byte[] data)
    {
        if (GameState.CurrentPhase == EPhase.PHASE_GAME)
        {
            return null;
        }
        
        var packet = PacketGCLoginSuccess4.Read(data);
        
        GameState.Characters = packet.Characters;
        
        var character = packet.Characters[0];
        
        Console.WriteLine($"Selecting character: {character.Name}");
        GameState.SelectedCharacter = character;
        
        GameState.BotId = await UpsertBotGettingId();
        
        // Console.WriteLine($"Game Port: {Configuration.GameServer.GamePort}");
        // await SocketHandler.GetInstance(null).ChangePortAsync(Configuration.GameServer.GamePort, true);

        // 

        TEA.EncryptKey =
        [
            GameState.RandomClientKey[0],
            GameState.RandomClientKey[1],
            GameState.RandomClientKey[2],
            GameState.RandomClientKey[3]
        ];
        
        return new BufferBuilder(2, true)
            .AddBytes([0x06, 0x00])
            .Build();
    }

    public async Task<Guid> UpsertBotGettingId()
    {
        var sql = @"
        INSERT INTO bots (
            nickname,
            region,
            last_packet_received_at
        ) VALUES (
            @Nickname,
            @Region,
            CURRENT_TIMESTAMP
        )
        ON CONFLICT (nickname) 
        DO UPDATE SET
            last_packet_received_at = CURRENT_TIMESTAMP
        RETURNING id;";
        
        var parameters = new Dictionary<string, object>
        {
            { "@Nickname", GameState.SelectedCharacter.Name },
            { "@Region", Configuration.Account.Region },
        };
        
        try
        {
            var id = await DatabaseService.ExecuteQueryReturningAsync<Guid>(sql, parameters);
            
            if (id == null)
            {
                throw new Exception("Error while upserting bot");
            }
            
            return id.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while upserting bot: {e.Message}");
            Environment.Exit(1);
            return Guid.Empty;
        }
    }
    
    // public int GetNewPort()
    // {
    //     if (alreadySwitched) return 0;
    //     alreadySwitched = true;
    //     return Configuration.GameServer.SelectPort;
    // }
}