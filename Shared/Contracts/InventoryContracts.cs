namespace MyApp.Shared.Contracts;

public record LowStockItemDto(
    int ProductId,
    string Sku,
    string Name,
    int OnHandQty,
    int ReorderLevel,
    string CategoryName);

public class InventorySummaryDto
{
    public int TotalProducts { get; set; }
    public int TotalOnHandUnits { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockCount { get; set; }
    public List<LowStockItemDto> LowStockItems { get; set; } = new();
}
