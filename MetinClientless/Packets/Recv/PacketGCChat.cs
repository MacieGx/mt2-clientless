using System.Buffers.Binary;
using System.Text;

namespace MetinClientless.Packets;

public struct PacketGCChat
{
    public EServerToClient Header;
    public EChatType Type;
    public string Message;
    
    public static PacketGCChat Read(byte[] buffer)
    {
        var size = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(1, 2));
        
        return new PacketGCChat
        {
            Header = (EServerToClient) buffer[0],
            Type = (EChatType) buffer[3],
            Message = Encoding.GetEncoding("Windows-1250").GetString(buffer, 9, size - 9),
        };
    }
}

public enum EChatType
{
    CHAT_TYPE_TALKING,
    CHAT_TYPE_INFO,
    CHAT_TYPE_NOTICE,
    CHAT_TYPE_PARTY,
    CHAT_TYPE_GUILD,
    CHAT_TYPE_COMMAND,
    CHAT_TYPE_SHOUT,
    CHAT_TYPE_WHISPER,
    CHAT_TYPE_BIG_NOTICE,
    CHAT_TYPE_MONARCH_NOTICE,
    CHAT_TYPE_DICE_INFO,
    CHAT_TYPE_MAX_NUM
};