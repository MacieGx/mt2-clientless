namespace MetinClientless.Packets;

public struct PacketGCPhase
{
    public byte Header;
    public EPhase Phase;
    
    public static PacketGCPhase Read(byte[] buffer)
    {
        return new PacketGCPhase
        {
            Header = buffer[0],
            Phase = (EPhase)buffer[1]
        };
    }
}

public enum EPhase {
    PHASE_CLOSE,
    PHASE_HANDSHAKE,
    PHASE_LOGIN,
    PHASE_SELECT,
    PHASE_LOADING,
    PHASE_GAME,
    PHASE_DEAD,
    PHASE_DBCLIENT_CONNECTING,
    PHASE_DBCLIENT,
    PHASE_P2P,
    PHASE_AUTH,
}