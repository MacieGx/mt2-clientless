using System.Text;

namespace MetinClientless.Packets;

public struct PacketGCLoginSuccess4
{
    public List<Character> Characters;
    
    public static PacketGCLoginSuccess4 Read(byte[] buffer)
    {
        var characters = new List<Character>();
        
        var dataBuffer = buffer[1..];
        for (int i = 0; i < Constants.MAX_CHARACTERS; i++)
        {
            var characterBuffer = dataBuffer[(i * Constants.SINGLE_CHARACTER_DATA_BYTES)..((i + 1) * Constants.SINGLE_CHARACTER_DATA_BYTES)];
            var character = Character.Read(characterBuffer);
            if (character.Id == 0) continue;
            characters.Add(character);
        }
        
        Console.WriteLine($"Received {characters.Count} characters");
        Console.WriteLine("Characters:");
        foreach (var character in characters)
        {
            Console.WriteLine($"Id: {character.Id}, CharacterName: {character.Name}");
        }
        
        return new PacketGCLoginSuccess4
        {
            Characters = characters
        };
    }
}


public struct Character
{
    public uint Id;
    public string Name;
    
    public static Character Read(byte[] buffer)
    {
        return new Character()
        {
            Id = BitConverter.ToUInt32(buffer, 0),
            Name = Encoding.ASCII.GetString(buffer[4..Constants.CHAR_NAME_MAX_LEN]).TrimEnd('\0')
        };
    }
}