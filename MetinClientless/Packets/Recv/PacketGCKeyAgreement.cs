namespace MetinClientless.Packets;

public struct PacketGCKeyAgreement
{
    public byte Header;
    public ushort agreedValueLength;
    public ushort publicKeyLength;
    public byte[] publicKey;
    
    public static PacketGCKeyAgreement Read(byte[] buffer)
    {
        return new PacketGCKeyAgreement
        {
            Header = buffer[0],
            agreedValueLength = BitConverter.ToUInt16(buffer, 1),
            publicKeyLength = BitConverter.ToUInt16(buffer, 3),
            publicKey = buffer[5..]
        };
    }
}