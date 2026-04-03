using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Domain;

public class PurchaseRequestDraftLine
{
    public int Id { get; set; }

    public int PurchaseRequestDraftId { get; set; }
    public PurchaseRequestDraft? PurchaseRequestDraft { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    [Range(0, int.MaxValue)]
    public int SuggestedQty { get; set; }

    [Range(1, int.MaxValue)]
    public int RequestedQty { get; set; }
}
