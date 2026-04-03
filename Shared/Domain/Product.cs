using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Shared.Domain;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int OnHandQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AverageCost { get; set; }

    public int ReorderLevel { get; set; } = 5;
    public int TargetStockLevel { get; set; } = 10;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    [MaxLength(450)]
    public string? DeletedByUserId { get; set; }

    [MaxLength(256)]
    public string? DeletedByUserName { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    public List<StockReceiptLine> ReceiptLines { get; set; } = new();
    public List<StockIssueLine> IssueLines { get; set; } = new();
    public List<StockAdjustmentLine> AdjustmentLines { get; set; } = new();
    public List<InventoryLedgerEntry> LedgerEntries { get; set; } = new();
}
