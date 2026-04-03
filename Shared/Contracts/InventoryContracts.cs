namespace MyApp.Shared.Contracts;

public static class InventoryMovementTypes
{
    public const string Receipt = "RECEIPT";
    public const string Issue = "ISSUE";
    public const string Adjustment = "ADJUSTMENT";

    public static readonly string[] All = [Receipt, Issue, Adjustment];
}

public static class ReorderRecommendationPriorities
{
    public const string Critical = "Critical";
    public const string High = "High";
    public const string Normal = "Normal";
}

public static class ReorderStockStatuses
{
    public const string OutOfStock = "OutOfStock";
    public const string Critical = "Critical";
    public const string Low = "Low";
    public const string Healthy = "Healthy";
}

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

public record StockCardEntryDto(
    int LedgerEntryId,
    DateTime OccurredAtUtc,
    string MovementType,
    string ReferenceNo,
    int QuantityChange,
    decimal UnitCost,
    decimal ValueChange,
    int RunningOnHandQty,
    decimal RunningAverageCost);

public class ProductStockCardDto
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int CurrentOnHandQty { get; set; }
    public decimal CurrentAverageCost { get; set; }
    public List<StockCardEntryDto> Entries { get; set; } = new();
}

public record ReorderRecommendationDto(
    int ProductId,
    string Sku,
    string ProductName,
    string CategoryName,
    int OnHandQty,
    int ReorderLevel,
    int TargetStockLevel,
    int SuggestedReorderQty,
    string Priority,
    string StockStatus);
