namespace MetinClientless.Handlers.Items;

public static class ItemSizeMap
{
    private static readonly Dictionary<int, int> _sizeMap;

    static ItemSizeMap()
    {
        _sizeMap = new Dictionary<int, int>();
        
        using var reader = new StreamReader("items.csv");
        // Read and ignore the header line
        var headerLine = reader.ReadLine();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrEmpty(line)) continue;
            
            var values = line.Split(',');
            if (values.Length >= 3 && int.TryParse(values[0], out int id) && int.TryParse(values[2], out int size))
            {
                _sizeMap[id] = size;
            }
        }
    }

    public static int GetItemSize(int itemId)
    {
        // vnum == 50300 || vnum == 70037
        var testId = itemId / 1000;
        if (testId == 50300 || testId == 70037)
        {
            return _sizeMap.TryGetValue(testId, out int size2) ? size2 : 0;
        }
        
        return _sizeMap.TryGetValue(itemId, out int size) ? size : 0;
    }
}