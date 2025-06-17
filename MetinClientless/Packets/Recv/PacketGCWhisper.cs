using System.Text;

namespace MetinClientless.Packets;

public class PacketGCWhisper
{
    public EServerToClient Header;
    public uint Size;
    public byte Type;
    public string NameFrom;
    public string Message;
    
    public static PacketGCWhisper Read(byte[] buffer)
    {
        return new PacketGCWhisper
        {
            Header = (EServerToClient) buffer[0],
            Size = BitConverter.ToUInt16(buffer, 1),
            Type = buffer[3],
            NameFrom = Encoding.GetEncoding("Windows-1250").GetString(buffer, 4, Constants.CHAR_NAME_MAX_LEN + 1).Split('\0')[0],
            Message = Encoding.GetEncoding("Windows-1250").GetString(buffer, 29, buffer.Length - 29).Trim('\0'),
        };
    }
    
}