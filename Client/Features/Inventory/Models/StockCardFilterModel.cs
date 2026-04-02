namespace MyApp.Client.Features.Inventory.Models;

public sealed class StockCardFilterModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? MovementType { get; set; }
}
