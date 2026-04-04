using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public static class PurchaseRequestDraftStatuses
{
    public const string Draft = "Draft";
    public const string Prepared = "Prepared";
    public const string Reviewed = "Reviewed";
}

public record PurchaseRequestDraftListDto(
    int Id,
    string DraftNo,
    string Status,
    DateTime CreatedAtUtc,
    string CreatedByUserName,
    int LineCount,
    DateTime? ReviewedAtUtc,
    string? ReviewedByUserName);

public record PurchaseRequestDraftLineDto(
    int Id,
    int ProductId,
    string ProductSku,
    string ProductName,
    int? SupplierId,
    string? SupplierName,
    int SuggestedQty,
    int RequestedQty);

public class PurchaseRequestDraftDetailDto
{
    public int Id { get; set; }
    public string DraftNo { get; set; } = string.Empty;
    public string Status { get; set; } = PurchaseRequestDraftStatuses.Draft;
    public DateTime CreatedAtUtc { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewedByUserName { get; set; }
    public string? Note { get; set; }
    public List<PurchaseRequestDraftLineDto> Lines { get; set; } = new();
}

public class CreatePurchaseRequestDraftRequest
{
    [MaxLength(500)]
    public string? Note { get; set; }

    [MinLength(1)]
    public List<CreatePurchaseRequestDraftLineRequest> Lines { get; set; } = new();
}

public class CreatePurchaseRequestDraftLineRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(0, int.MaxValue)]
    public int SuggestedQty { get; set; }

    [Range(1, int.MaxValue)]
    public int RequestedQty { get; set; }
}

public class UpdatePurchaseRequestDraftLineRequest
{
    [Range(1, int.MaxValue)]
    public int RequestedQty { get; set; }

    public int? SupplierId { get; set; }
}
