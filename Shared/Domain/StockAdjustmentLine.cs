using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Shared.Domain;

public class StockAdjustmentLine
{
    public int Id { get; set; }

    public int StockAdjustmentId { get; set; }
    public StockAdjustment? StockAdjustment { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    [MaxLength(20)]
    public string Direction { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }
}
