using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Shared.Domain;

public class InventoryLedgerEntry
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    [MaxLength(20)]
    public string MovementType { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ReferenceNo { get; set; } = string.Empty;

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public int QuantityChange { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValueChange { get; set; }

    public int RunningOnHandQty { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RunningAverageCost { get; set; }
}
