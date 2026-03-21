using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Shared.Domain;

public class StockIssue
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string DocumentNo { get; set; } = string.Empty;

    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? Customer { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public List<StockIssueLine> Lines { get; set; } = new();
}
