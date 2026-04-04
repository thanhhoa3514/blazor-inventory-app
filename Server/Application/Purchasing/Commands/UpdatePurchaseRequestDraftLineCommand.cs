using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Commands;

public sealed class UpdatePurchaseRequestDraftLineCommand
{
    private readonly IPurchaseRequestDraftRepository _drafts;
    private readonly ISupplierRepository _suppliers;

    public UpdatePurchaseRequestDraftLineCommand(IPurchaseRequestDraftRepository drafts, ISupplierRepository suppliers)
    {
        _drafts = drafts;
        _suppliers = suppliers;
    }

    public async Task<AppResult<PurchaseRequestDraftDetailDto>> ExecuteAsync(
        int draftId,
        int lineId,
        UpdatePurchaseRequestDraftLineRequest request,
        CancellationToken ct = default)
    {
        var draft = await _drafts.FindAsync(draftId, ct);
        if (draft is null)
            return new AppResult<PurchaseRequestDraftDetailDto>.NotFound($"Draft {draftId} not found.");

        if (!string.Equals(draft.Status, PurchaseRequestDraftStatuses.Draft, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(draft.Status, PurchaseRequestDraftStatuses.Prepared, StringComparison.OrdinalIgnoreCase))
            return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Reviewed purchase requests cannot be edited.");

        var line = draft.Lines.FirstOrDefault(x => x.Id == lineId);
        if (line is null)
            return new AppResult<PurchaseRequestDraftDetailDto>.NotFound($"Draft line {lineId} not found.");

        if (request.SupplierId.HasValue)
        {
            var supplier = await _suppliers.FindActiveAsync(request.SupplierId.Value, ct);
            if (supplier is null)
                return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Supplier does not exist or is inactive.");
        }

        line.RequestedQty = request.RequestedQty;
        line.SupplierId = request.SupplierId;
        await _drafts.SaveChangesAsync(ct);

        var dto = await _drafts.GetByIdAsync(draftId, ct);
        return new AppResult<PurchaseRequestDraftDetailDto>.Ok(dto!);
    }
}
