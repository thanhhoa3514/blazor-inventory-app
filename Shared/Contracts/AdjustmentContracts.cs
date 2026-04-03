using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public static class StockAdjustmentDirections
{
    public const string Increase = "increase";
    public const string Decrease = "decrease";
}

public record StockAdjustmentListDto(
    int Id,
    string DocumentNo,
    DateTime AdjustedAtUtc,
    string Reason,
    decimal TotalAmount,
    int LineCount);

public record StockAdjustmentDetailLineDto(
    int ProductId,
    string ProductSku,
    string ProductName,
    string Direction,
    int Quantity,
    decimal UnitCost,
    decimal LineTotal);

public class StockAdjustmentDetailDto
{
    public int Id { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateTime AdjustedAtUtc { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public List<StockAdjustmentDetailLineDto> Lines { get; set; } = new();
}

public class CreateStockAdjustmentRequest
{
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    [MinLength(1)]
    public List<CreateStockAdjustmentLineRequest> Lines { get; set; } = new();
}

public class CreateStockAdjustmentLineRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Direction { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
