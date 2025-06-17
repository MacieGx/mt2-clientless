using MetinClientless.Handlers;
using MetinClientless.Handlers.Items;

public class Placement
{
    public Guid ItemGuid { get; set; }
    public int InventoryPosition { get; set; }
    public int CellIndex { get; set; }
    public int ItemSize { get; set; }
}


public class TradeItem
{
    public Guid ItemGuid { get; set; }
    public int Index { get; set; }
    public int Size { get; set; }
}

public class TradeWindowHelper
{
    private const int WINDOW_WIDTH = 4;
    private const int WINDOW_HEIGHT = 3;
    private const int TOTAL_CELLS = WINDOW_WIDTH * WINDOW_HEIGHT;

    public List<Placement> OptimizePlacement(List<PlayerItem> playerItems)
    {
        var placements = new List<Placement>();
        var occupiedCells = new bool[TOTAL_CELLS];

        // Convert and filter items
        var items = playerItems
            .Select(i => new TradeItem
            {
                ItemGuid = i.Id,
                Index = i.InventoryPosition,
                Size = ItemSizeMap.GetItemSize(i.ItemId)
            })
            .Where(i => i.Size <= WINDOW_HEIGHT)
            .ToList();

        // Try to place each item
        foreach (var item in items)
        {
            bool placed = false;
            // Try each cell until we find a valid spot
            for (int cell = 0; cell < TOTAL_CELLS && !placed; cell++)
            {
                if (CanPlaceItem(cell, item.Size, occupiedCells))
                {
                    // Add placement
                    placements.Add(new Placement
                    {
                        ItemGuid = item.ItemGuid,
                        CellIndex = cell,
                        InventoryPosition = item.Index,
                        ItemSize = item.Size
                    });

                    // Mark cells as occupied
                    for (int i = 0; i < item.Size; i++)
                    {
                        occupiedCells[cell + (WINDOW_WIDTH * i)] = true;
                    }
                    placed = true;
                }
            }
            
            if (!placed)
            {
                // If we couldn't place an item, return what we have so far
                return placements;
            }
        }

        return placements;
    }

    private bool CanPlaceItem(int cellIndex, int size, bool[] occupiedCells)
    {
        // Check if starting position is valid for item size
        if (size == 3 && cellIndex >= WINDOW_WIDTH) return false;
        if (size == 2 && cellIndex >= WINDOW_WIDTH * (WINDOW_HEIGHT - 1)) return false;

        // Check if all needed cells are free
        for (int i = 0; i < size; i++)
        {
            int cell = cellIndex + (WINDOW_WIDTH * i);
            if (cell >= TOTAL_CELLS || occupiedCells[cell]) return false;
        }
        return true;
    }
}