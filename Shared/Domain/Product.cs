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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    public List<StockReceiptLine> ReceiptLines { get; set; } = new();
    public List<StockIssueLine> IssueLines { get; set; } = new();
    public List<InventoryLedgerEntry> LedgerEntries { get; set; } = new();
}
