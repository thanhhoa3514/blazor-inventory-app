using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public record StockIssueListDto(
    int Id,
    string DocumentNo,
    DateTime IssuedAtUtc,
    int? CustomerId,
    string? Customer,
    decimal TotalAmount,
    int LineCount);

public record StockIssueDetailLineDto(
    int ProductId,
    string ProductSku,
    string ProductName,
    int Quantity,
    decimal UnitCost,
    decimal LineTotal);

public class StockIssueDetailDto
{
    public int Id { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateTime IssuedAtUtc { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? Customer { get; set; }
    public string? Note { get; set; }
    public decimal TotalAmount { get; set; }
    public List<StockIssueDetailLineDto> Lines { get; set; } = new();
}

public class CreateStockIssueRequest
{
    public int? CustomerId { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [MinLength(1)]
    public List<CreateStockIssueLineRequest> Lines { get; set; } = new();
}

public class CreateStockIssueLineRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
