using System.Text;

namespace MetinClientless.Packets;

public struct PacketGCCharAdditionalInfo
{
    public EServerToClient Header;
    public uint Id;
    public string Name;
    
    public static PacketGCCharAdditionalInfo Read(byte[] buffer)
    {
        return new PacketGCCharAdditionalInfo
        {
            Header = (EServerToClient) BitConverter.ToUInt16(buffer, 0),
            Id = BitConverter.ToUInt32(buffer, 1),
            Name = Encoding.ASCII.GetString(buffer, 5, Constants.CHAR_NAME_MAX_LEN + 1).TrimEnd('\0')
        };
    }
}