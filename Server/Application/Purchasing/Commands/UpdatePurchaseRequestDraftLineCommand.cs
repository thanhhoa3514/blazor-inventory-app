using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Commands;

public sealed class UpdatePurchaseRequestDraftLineCommand
{
    private readonly IPurchaseRequestDraftRepository _drafts;

    public UpdatePurchaseRequestDraftLineCommand(IPurchaseRequestDraftRepository drafts)
    {
        _drafts = drafts;
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

        if (!string.Equals(draft.Status, PurchaseRequestDraftStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Only draft purchase requests can be edited.");

        var line = draft.Lines.FirstOrDefault(x => x.Id == lineId);
        if (line is null)
            return new AppResult<PurchaseRequestDraftDetailDto>.NotFound($"Draft line {lineId} not found.");

        line.RequestedQty = request.RequestedQty;
        await _drafts.SaveChangesAsync(ct);

        var dto = await _drafts.GetByIdAsync(draftId, ct);
        return new AppResult<PurchaseRequestDraftDetailDto>.Ok(dto!);
    }
}
