namespace MetinClientless.Handlers;

public class PlayerItem
{
    public Guid Id { get; set; }
    public Guid BotId { get; set; }
    public string PlayerName { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
    public int InventoryPosition { get; set; }
    public bool Withdrawn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}