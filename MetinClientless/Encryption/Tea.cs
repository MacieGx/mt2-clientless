using System;

public class TEA
{
    private const int ROUND = 32;
    private const uint DELTA = 0x9E3779B9;

    // You need to find decryption and encryption key by yourself
    public static uint[] EncryptKey = [0, 0, 0, 0];
    public static uint[] DecryptKey = [0, 0, 0, 0];
    
    private static void Code(uint sz, uint sy, uint[] key, uint[] dest)
    {
        unchecked
        {
            uint y = sy, z = sz;
            uint sum = 0;
            int n = ROUND;

            while (n-- > 0)
            {
                y += ((z << 4 ^ z >> 5) + z) ^ (sum + key[sum & 3]);
                sum += DELTA;
                z += ((y << 4 ^ y >> 5) + y) ^ (sum + key[sum >> 11 & 3]);
            }

            dest[0] = y;
            dest[1] = z;
        }
    }

    private static void Decode(uint sz, uint sy, uint[] key, uint[] dest)
    {
        unchecked
        {
            uint y = sy, z = sz;
            ulong sum = (ulong)DELTA * (ulong)ROUND;
            int n = ROUND;

            while (n-- > 0)
            {
                z -= ((y << 4 ^ y >> 5) + y) ^ ((uint)sum + key[sum >> 11 & 3]);
                sum -= DELTA;
                y -= ((z << 4 ^ z >> 5) + z) ^ ((uint)sum + key[sum & 3]);
            }

            dest[0] = y;
            dest[1] = z;
        }
    }

    private static unsafe uint BytesToUInt32(byte[] bytes, int startIndex)
    {
        fixed (byte* ptr = &bytes[startIndex])
        {
            return *(uint*)ptr;
        }
    }

    private static unsafe void UInt32ToBytes(uint value, byte[] bytes, int startIndex)
    {
        fixed (byte* ptr = &bytes[startIndex])
        {
            *(uint*)ptr = value;
        }
    }

    public static unsafe byte[] Encrypt(byte[] input, uint[]? customEncryptKey)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentException("Input cannot be null or empty");

        int size = input.Length;
        int resize = size;
        
        if (size % 8 != 0)
        {
            resize = size + (8 - (size % 8));
        }

        byte[] paddedInput = new byte[resize];
        Buffer.BlockCopy(input, 0, paddedInput, 0, size);
        
        byte[] output = new byte[resize];

        fixed (byte* pInput = paddedInput)
        fixed (byte* pOutput = output)
        {
            for (int i = 0; i < resize >> 3; i++)
            {
                uint[] source = new uint[2];
                uint[] dest = new uint[2];
                
                source[0] = BytesToUInt32(paddedInput, i * 8);
                source[1] = BytesToUInt32(paddedInput, i * 8 + 4);
                
                Code(source[1], source[0], customEncryptKey ?? EncryptKey, dest);
                
                UInt32ToBytes(dest[0], output, i * 8);
                UInt32ToBytes(dest[1], output, i * 8 + 4);
            }
        }

        return output;
    }

    public static unsafe byte[] Decrypt(byte[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentException("Input cannot be null or empty");

        int size = input.Length;
        byte[] output = new byte[size];

        fixed (byte* pInput = input)
        fixed (byte* pOutput = output)
        {
            for (int i = 0; i < size >> 3; i++)
            {
                uint[] source = new uint[2];
                uint[] dest = new uint[2];
                
                source[0] = BytesToUInt32(input, i * 8);
                source[1] = BytesToUInt32(input, i * 8 + 4);
                
                Decode(source[1], source[0], DecryptKey, dest);
                
                UInt32ToBytes(dest[0], output, i * 8);
                UInt32ToBytes(dest[1], output, i * 8 + 4);
            }
        }

        return output;
    }
}