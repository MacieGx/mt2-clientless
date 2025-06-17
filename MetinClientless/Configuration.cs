using MetinClientless.Errors;

namespace MetinClientless;

public static class Configuration
{
    public static GameServer GameServer => _gameServer ??= InitializeGameServer();
    private static GameServer? _gameServer;

    public static Account Account => _account ??= InitializeAccount();
    private static Account? _account;

    public static Proxy Proxy => _proxy ??= InitializeProxy();
    private static Proxy? _proxy;

    public static string PostgresConnectionString =>
        GetEnvVariable("POSTGRES_CONNECTION_STRING") + ";Multiplexing=true;Enlist=false;";
    
    public static readonly string[] TradeAllowedCharacters = new[]
    {
        "Nickname1"
    };

    private static GameServer InitializeGameServer() => new GameServer
    {
        IP = GetEnvVariable("SERVER_IP"),
        GateKeeperIP = GetEnvVariable("GATEKEEPER_IP"),
        Version = uint.Parse(GetEnvVariable("CLIENT_VERSION")),
        LoginPort = GetEnvVariableAsInt("LOGIN_PORT"),
        SelectPort = GetEnvVariableAsInt("SELECT_PORT"),
        GamePort = GetEnvVariableAsInt("GAME_PORT"),
        SendSequence = GetEnvVariableAsBool("SEND_SEQUENCE")
    };

    private static Account InitializeAccount() => new Account
    {
        Username = GetEnvVariable("ACCOUNT_USERNAME"),
        Password = GetEnvVariable("ACCOUNT_PASSWORD"),
        Pin = GetEnvVariable("ACCOUNT_PIN"),
        Region = GetEnvVariable("ACCOUNT_REGION"),
        HwidHash = GetEnvVariable("HWID_HASH"),
        ShopIdModulo = GetEnvVariableAsInt("SHOP_ID_MODULO"),
        ShopIdExpectedModuloValue = GetEnvVariableAsInt("SHOP_ID_EXPECTED_MODULO_VALUE")
    };

    private static Proxy InitializeProxy()
    {
        var enabled = bool.Parse(GetEnvVariable("PROXY_ENABLED"));

        if (!enabled)
        {
            return new Proxy()
            {
                Enabled = false
            };
        }
        
        return new Proxy
        {
            Enabled = enabled,
            Host = GetEnvVariable("PROXY_HOST"),
            Port = GetEnvVariableAsInt("PROXY_PORT"),
            Username = GetEnvVariable("PROXY_USERNAME"),
            Password = GetEnvVariable("PROXY_PASSWORD")
        };
    }

    private static bool GetEnvVariableAsBool(string key) =>
        bool.Parse(Environment.GetEnvironmentVariable(key) ?? "false");

    private static string GetEnvVariable(string key) =>
        Environment.GetEnvironmentVariable(key) ?? throw new MissingEnvironmentVariableException(key);

    private static int GetEnvVariableAsInt(string key) =>
        int.TryParse(Environment.GetEnvironmentVariable(key), out int value)
            ? value
            : throw new InvalidOperationException($"{key} must be a valid integer");
}


public struct GameServer
{
    public string IP;
    public string GateKeeperIP;
    public uint Version;
    public int LoginPort;
    public int SelectPort;
    public int GamePort;
    public bool SendSequence;
}

public struct Account
{
    public string Username;
    public string Password;
    public string Pin;
    public string Region;
    public string HwidHash;
    public int ShopIdModulo;
    public int ShopIdExpectedModuloValue;
}

public struct Proxy
{
    public bool Enabled;
    public string Host;
    public int Port;
    public string Username;
    public string Password;
}