using System.Buffers.Binary;
using System.Text;
using System.Text.Json;

namespace MetinClientless.Packets;

public struct PacketGCShopContent
{
    public uint Id;
    public uint ShopTransactionId;
    public string Name;
    public string PlayerName;
    public byte ItemsCount;
    public List<ShopItem> Items;
    
    public static PacketGCShopContent Read(byte[] buffer)
    {
        var itemsCount = buffer[67];
        
        var ITEMS_PADDING = 86;
        
        return new PacketGCShopContent
        {
            Id = BitConverter.ToUInt32(buffer, 6),
            ShopTransactionId = BitConverter.ToUInt32(buffer, 11),
            Name = Encoding.ASCII.GetString(buffer[19..(19 + Constants.SHOP_NAME_MAX)]).TrimEnd('\0'),
            PlayerName = Encoding.ASCII.GetString(buffer[52..67]).TrimEnd('\0'),
            ItemsCount = itemsCount,
            Items = ReadItems(buffer[ITEMS_PADDING..], itemsCount)
        };
    }
    
    private static List<ShopItem> ReadItems(byte[] buffer, byte itemsCount)
    {
        var SINGLE_ITEM_BYTES = 64;
        
        var items = new List<ShopItem>();
        
        for (int i = 0; i < itemsCount; i++)
        {
            var itemBuffer = buffer[(i * SINGLE_ITEM_BYTES)..((i + 1) * SINGLE_ITEM_BYTES)];
            
            var item = ShopItem.Read(itemBuffer);
            
            items.Add(item);
        }
        
        items.Sort((a, b) => a.Pos.CompareTo(b.Pos));

        return items;
    }
}

public struct ShopItem
{
    public uint ItemTransactionId;
    public uint Vnum;
    public uint Count;
    public bool HasBuiltInAttribute;
    public List<ItemAttribute> Attributes;
    public List<uint> Sockets;
    public uint Price;
    public uint Pos;
    
    public static ShopItem Read(byte[] buffer)
    {
        uint vnum = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(11));

        if (vnum == 50300 || vnum == 70037)
        {
            int spellId = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(15));
            
            vnum = vnum * 1000;
            vnum += (uint)spellId;
        }

        return new ShopItem
        {
            ItemTransactionId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(0)),
            Pos = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(5)), 
            Count = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(7)),
            Sockets = ItemSocket.ReadAll(buffer[15..]).Select(x => x.SocketId).ToList(),
            HasBuiltInAttribute = buffer[42] != 0,
            Attributes = ItemAttribute.ReadAll(buffer[27..]),
            Price = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan(56)),
            Vnum = vnum
        };
    }
    
    public string BuildAttributesJson()
    {
        var attributeList = new List<Dictionary<string, short>>();

        foreach (var attribute in Attributes)
        {
            attributeList.Add(new Dictionary<string, short>
            {
                { "type", attribute.Type },
                { "value", attribute.Value }
            });
        }

        return JsonSerializer.Serialize(attributeList, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }
    
    public string BuildSocketsJson()
    {
        return JsonSerializer.Serialize(Sockets, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }
}

public struct ItemAttribute
{
    public byte Type;
    public short Value;
    
    public static List<ItemAttribute> ReadAll(byte[] buffer)
    {
        var attributes = new List<ItemAttribute>();
        
        var additionalAttributesPadding = Constants.ITEM_MAX_ATTRIBUTES * 3;
        var builtInAttributeType = buffer[additionalAttributesPadding];
        var builtInAttributeValue = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(1 + additionalAttributesPadding));

        if (builtInAttributeType != 0 && builtInAttributeValue != 0)
        {
            attributes.Add(new ItemAttribute
            {
                Type = builtInAttributeType,
                Value = builtInAttributeValue
            });
        }
        
        for (int i = 0; i < Constants.ITEM_MAX_ATTRIBUTES; i++)
        {
            var offset = i * 3;
            
            var type = buffer[offset];
            var value = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(1 + offset));
            
            if (type == 0 || value == 0)
                break;
            
            attributes.Add(new ItemAttribute
            {
                Type = type,
                Value = value
            });
        }
        
        return attributes;
    }
}

public struct ItemSocket
{
    public uint SocketId;
    
    public static ItemSocket Read(byte[] buffer)
    {
        return new ItemSocket
        {
            SocketId = BitConverter.ToUInt32(buffer, 0)
        };
    }
    
    public static List<ItemSocket> ReadAll(byte[] buffer)
    {
        var sockets = new List<ItemSocket>();
        
        for (int i = 0; i < Constants.ITEM_MAX_SOCKETS; i++)
        {
            var offset = i * 4;
            
            var socketId = BitConverter.ToUInt32(buffer, offset);
            
            sockets.Add(new ItemSocket
            {
                SocketId = socketId
            });
        }
        
        return sockets;
    }
}