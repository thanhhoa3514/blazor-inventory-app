using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Domain;

public class PurchaseRequestDraft
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string DraftNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Draft";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [MaxLength(450)]
    public string? CreatedByUserId { get; set; }

    [Required]
    [MaxLength(256)]
    public string CreatedByUserName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    public List<PurchaseRequestDraftLine> Lines { get; set; } = new();
}
