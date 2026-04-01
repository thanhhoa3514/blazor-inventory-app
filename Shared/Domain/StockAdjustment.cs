using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Shared.Domain;

public class StockAdjustment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string DocumentNo { get; set; } = string.Empty;

    public DateTime AdjustedAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public List<StockAdjustmentLine> Lines { get; set; } = new();
}
