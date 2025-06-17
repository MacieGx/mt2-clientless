using System.Text;
using MetinClientless;
using MetinClientless.Handlers;
using MetinClientless.Packets;
using DotNetEnv;
using MetinClientless.Services;

Env.Load();

var registry = new PacketHandlerRegistry();

registry.RegisterHandler(EServerToClient.HEADER_GC_PHASE, new PhaseChangeHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_HANDSHAKE, new HandshakeHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_KEY_AGREEMENT, new KeyAgreementHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_KEY_AGREEMENT_COMPLETED, new KeyAgreementCompletedHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_AUTH_SUCCESS, new AuthSuccessHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_LOGIN_SUCCESS4, new LoginSuccessHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_LOGIN_FAILURE, new LoginFailureHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_MT2009_SHOP_DATA, new ShopDataHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_PING, new PingHandler());   
registry.RegisterHandler(EServerToClient.HEADER_GC_CHAR_ADDITIONAL_INFO, new CharAdditionalInfoHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_CHARACTER_DEL, new CharacterDelHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_EXCHANGE, new ExchangeHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_CHAT, new ChatHandler());
registry.RegisterHandler(EServerToClient.HEADER_GC_WHISPER, new WhisperHandler());

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


var socketHandler = SocketHandler.GetInstance(registry);

var socketHandlerTask = async () =>
{
    try
    {
        // socketHandler.SendGatekeeperRequest();
        await socketHandler.ConnectAsync(Configuration.GameServer.IP, Configuration.GameServer.LoginPort);  
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        Environment.Exit(1);
    }
};


var shopScannerTask = async () =>
{
    try
    {
        await new ShopHandler(socketHandler).Handle();
    } catch (Exception e)
    {
        Console.WriteLine(e);
        Environment.Exit(1);
    }
};

var loginTimeoutTask = async () =>
{
    var taskStartedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    while (true)
    {
        await Task.Delay(1000);
        if (GameState.CurrentPhase == EPhase.PHASE_GAME)
        {
            break;
        }

        if (taskStartedAt + 10_000 < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds())
        {
            Console.WriteLine("Login Timeout");
            Environment.Exit(1);
        }
    }
};

var buyRequestHandler = async () =>
{
    try
    {
        await new BuyRequestHandler(socketHandler).Handle();
    } catch (Exception e)
    {
        Console.WriteLine(e);
        Environment.Exit(1);
    }
};



await Task.WhenAll(Task.Run(socketHandlerTask), Task.Run(shopScannerTask), Task.Run(loginTimeoutTask), Task.Run(buyRequestHandler));