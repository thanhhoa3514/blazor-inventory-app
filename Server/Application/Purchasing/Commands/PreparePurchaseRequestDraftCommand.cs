using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Commands;

public sealed class PreparePurchaseRequestDraftCommand
{
    private readonly IPurchaseRequestDraftRepository _drafts;

    public PreparePurchaseRequestDraftCommand(IPurchaseRequestDraftRepository drafts)
    {
        _drafts = drafts;
    }

    public async Task<AppResult<PurchaseRequestDraftDetailDto>> ExecuteAsync(int draftId, CancellationToken ct = default)
    {
        var draft = await _drafts.FindAsync(draftId, ct);
        if (draft is null)
            return new AppResult<PurchaseRequestDraftDetailDto>.NotFound($"Draft {draftId} not found.");

        if (!string.Equals(draft.Status, PurchaseRequestDraftStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Only draft purchase requests can be marked as prepared.");

        if (draft.Lines.Count == 0)
            return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Draft must contain at least one line before it can be prepared.");

        draft.Status = PurchaseRequestDraftStatuses.Prepared;
        draft.ReviewedAtUtc = null;
        draft.ReviewedByUserId = null;
        draft.ReviewedByUserName = null;
        await _drafts.SaveChangesAsync(ct);

        var dto = await _drafts.GetByIdAsync(draftId, ct);
        return new AppResult<PurchaseRequestDraftDetailDto>.Ok(dto!);
    }
}
