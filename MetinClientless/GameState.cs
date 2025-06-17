using System.Security.Cryptography;
using MetinClientless.Packets;

namespace MetinClientless;

public class GameState
{
    public static EPhase CurrentPhase { get; set; } = EPhase.PHASE_HANDSHAKE;
    public static uint LoginKey { get; set; }
    public static List<Character> Characters { get; set; } = [];
    public static Character SelectedCharacter { get; set; }
    public static Guid BotId { get; set; } = Guid.Empty;
    public static List<uint> OpenShopIds { get; set; } = [];
    public static List<uint> ScanShopIds { get; set; } = [];
    public static bool ShopScanningPaused = false;
    public static long ShopScanningPausedEndTimestampMs { get; set; } = 0;
    public static bool IsTradeWindowOpen = false;
    public static bool IsBuyingActionInProgress = false;
    public static bool IsWaitingForShopRecv { get; set; } = false;
    public static Dictionary<uint, string> PlayerNameMap { get; set; } = new();

    public static uint[] RandomClientKey { get; set; } = new uint[4]
    {
        1000000000 + (uint)RandomNumberGenerator.GetInt32(0, 900000000),
        (uint)RandomNumberGenerator.GetInt32(100000000, 1000000000),
        1000000000 + (uint)RandomNumberGenerator.GetInt32(0, 900000000),
        1000000000 + (uint)RandomNumberGenerator.GetInt32(0, 900000000),
    };
}