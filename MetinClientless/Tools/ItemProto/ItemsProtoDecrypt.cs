using System.IO.Compression;
using System.Text;
using lzo.net;

namespace MetinClientless.Tools;

public static class ItemsProtoDecrypt
{
    
    public static void Decrypt(string encryptedFilePath, string decryptedFilePath)
    {
        // You need to find decryption key by yourself
        TEA.DecryptKey =
        [
            0,
            0,
            0,
            0
        ];
        
        var encryptedBytes = File.ReadAllBytes(encryptedFilePath);
        
        var dwFourCC = BitConverter.ToUInt32(encryptedBytes, 0);
        
        if (dwFourCC == MakeFourCC('M', 'I', 'P', 'X'))
        {
            var dwVersion = BitConverter.ToUInt32(encryptedBytes, 4);
            
            if (dwVersion != 1)
            {
                throw new Exception($"Invalid item proto version: {dwVersion}");
            }
        } else if (dwFourCC != MakeFourCC('M', 'I', 'P', 'T'))
        {
            throw new Exception("Invalid item proto type");
        }
        
        var dwStride = BitConverter.ToUInt32(encryptedBytes, 8);
        Console.WriteLine($"Item Proto Stride: {dwStride}");
        
        
        var dwElements = BitConverter.ToUInt32(encryptedBytes, 12);
        var dwSize = BitConverter.ToUInt32(encryptedBytes, 16);
        
        Console.WriteLine($"Item Proto Elements: {dwElements}");
        Console.WriteLine($"Item Proto Size: {dwSize}");
        
        var pbData = encryptedBytes[20..(int)dwSize];
        var decrypted = TEA.Decrypt(pbData);
        
        using (var compressedStream = new MemoryStream(decrypted))
        using (var lzo = new LzoStream(compressedStream, CompressionMode.Decompress))
        {
            var decompressedBytes = new byte[decrypted.Length * 10];
            var sizeRead = lzo.Read(decompressedBytes, 0, (int)dwElements * (int)dwStride);
            File.WriteAllBytes(decryptedFilePath, decompressedBytes[..sizeRead]);

            
            for (int i = 0; i < dwElements; i++)
            {
                var itemChunk = decompressedBytes[(i * (int)dwStride)..((i + 1) * (int)dwStride)];
                var dwIndex = BitConverter.ToUInt32(itemChunk, 0);
                if (dwIndex == 0)
                {
                    continue;
                }
                
                var vnum = BitConverter.ToUInt32(itemChunk, 61);
                if (vnum == 0)
                {
                    continue;
                }
                
                var ITEM_NAME_MAX_LEN = 24;
                
                var name = Encoding.GetEncoding("Windows-1250").GetString(itemChunk[111..(111 + ITEM_NAME_MAX_LEN + 1)]).TrimEnd('\0');
                int type = itemChunk[144];
                int subtype = itemChunk[145];
                int size = itemChunk[147];
                
                Console.WriteLine($"Vnum: {vnum}, Name: {name}, Type: {type}, Subtype: {subtype}, Size: {size}");
            }
        }
    }
    
    private static uint MakeFourCC(char ch0, char ch1, char ch2, char ch3)
    {
        return (uint)((byte)ch0 | ((byte)ch1 << 8) | ((byte)ch2 << 16) | ((byte)ch3 << 24));
    }
}