using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public record StockReceiptListDto(
    int Id,
    string DocumentNo,
    DateTime ReceivedAtUtc,
    string? Supplier,
    decimal TotalAmount,
    int LineCount);

public record StockReceiptDetailLineDto(
    int ProductId,
    string ProductSku,
    string ProductName,
    int Quantity,
    decimal UnitCost,
    decimal LineTotal);

public class StockReceiptDetailDto
{
    public int Id { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; set; }
    public string? Supplier { get; set; }
    public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public List<StockReceiptDetailLineDto> Lines { get; set; } = new();
}

public class CreateStockReceiptRequest
{
    [MaxLength(200)]
    public string? Supplier { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [MinLength(1)]
    public List<CreateStockReceiptLineRequest> Lines { get; set; } = new();
}

public class CreateStockReceiptLineRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal UnitCost { get; set; }
}
