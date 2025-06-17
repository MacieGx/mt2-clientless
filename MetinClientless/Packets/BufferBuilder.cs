using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace MetinClientless.Packets;

public class BufferBuilder
{
    private bool SendSequence;
    private byte[] _buffer;
    private int _index;

    public BufferBuilder(int size, bool sendSequence = false)
    {
        if (Configuration.GameServer.SendSequence && sendSequence)
        {
            size++;
        };
        
        _buffer = new byte[size];
    }

    public BufferBuilder AddByte(byte value, int repeat = 1)
    {
        for (int i = 0; i < repeat; i++)
        {
            _buffer[_index++] = value;
        }

        return this;
    }
    
    public BufferBuilder AddBytes(byte[] value)
    {
        value.CopyTo(_buffer, _index);
        _index += value.Length;
        return this;
    }

    public BufferBuilder AddString(string value, int maxLength)
    {
        var bytes = Encoding.ASCII.GetBytes(value);

        if (bytes.Length > maxLength)
        {
            throw new Exception($"{value} is too long");
        }

        Array.Copy(bytes, 0, _buffer, _index, bytes.Length);
        Array.Fill(_buffer, (byte)0, _index + bytes.Length, maxLength - bytes.Length);

        _index += maxLength;
        return this;
    }
    
    public BufferBuilder AddUInt16(ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(_buffer.AsSpan(_index), value);
        _index += 2;
        return this;
    }
    
    public BufferBuilder AddUInt32(uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(_buffer.AsSpan(_index), value);
        _index += 4;
        return this;
    }
    
    public BufferBuilder AddUInt64(ulong value)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(_buffer.AsSpan(_index), value);
        _index += 8;
        return this;
    }
    
    public BufferBuilder AddInt16(short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(_buffer.AsSpan(_index), value);
        _index += 2;
        return this;
    }
    
    public BufferBuilder AddInt32(int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(_buffer.AsSpan(_index), value);
        _index += 4;
        return this;
    }
    
    
    public BufferBuilder AddRandomSha256(bool upperCase = false)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[32];
            rng.GetBytes(randomBytes);  // Fill the array with random bytes

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(randomBytes);
                var ansiString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                
                return AddString(upperCase ? ansiString.ToUpper() : ansiString, 64);
            }
        }
    }

    public byte[] Build()
    {
        if (Configuration.GameServer.SendSequence && SendSequence)
        {
            _buffer[^1] = SequenceTable.GetNextByte();
        }
        
        return _buffer;
    }
}