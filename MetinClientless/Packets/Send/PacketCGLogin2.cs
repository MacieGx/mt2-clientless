using System.Text;

namespace MetinClientless.Packets.Send;

public class PacketCGLogin2
{
    public static byte[] Serialize(string username)
    {
       var clientKeysBytes = GameState.RandomClientKey.SelectMany(BitConverter.GetBytes).ToArray();

       // You need to find custom encryption key by yourself
       byte[] decrypted = TEA.Encrypt(clientKeysBytes, [
           0,
           0,
           0,
           0
       ]);
       
       uint[] newDecryptKey = new uint[4];
        for (int i = 0; i < 4; i++)
        {
            newDecryptKey[i] = BitConverter.ToUInt32(decrypted, i * 4);
        }
        
        TEA.DecryptKey = newDecryptKey;
        
        
        return new BufferBuilder(52, true)
            .AddByte((byte)EClientToServer.HEADER_CG_LOGIN2)
            .AddString(username, Constants.ID_MAX_NUM)
            .AddByte(0x00)
            .AddUInt32(GameState.LoginKey)
            .AddUInt32(GameState.RandomClientKey[0])
            .AddUInt32(GameState.RandomClientKey[1])
            .AddUInt32(GameState.RandomClientKey[2])
            .AddUInt32(GameState.RandomClientKey[3])
            .Build();
    }
}