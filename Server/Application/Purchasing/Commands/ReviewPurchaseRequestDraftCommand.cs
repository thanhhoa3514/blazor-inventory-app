using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Commands;

public sealed class ReviewPurchaseRequestDraftCommand
{
    private readonly IPurchaseRequestDraftRepository _drafts;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public ReviewPurchaseRequestDraftCommand(
        IPurchaseRequestDraftRepository drafts,
        ICurrentUserAccessor currentUserAccessor)
    {
        _drafts = drafts;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<AppResult<PurchaseRequestDraftDetailDto>> ExecuteAsync(int draftId, CancellationToken ct = default)
    {
        var draft = await _drafts.FindAsync(draftId, ct);
        if (draft is null)
            return new AppResult<PurchaseRequestDraftDetailDto>.NotFound($"Draft {draftId} not found.");

        if (!string.Equals(draft.Status, PurchaseRequestDraftStatuses.Prepared, StringComparison.OrdinalIgnoreCase))
            return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Only prepared purchase requests can be reviewed.");

        var currentUser = _currentUserAccessor.GetRequiredCurrentUser();
        draft.Status = PurchaseRequestDraftStatuses.Reviewed;
        draft.ReviewedAtUtc = DateTime.UtcNow;
        draft.ReviewedByUserId = currentUser.UserId;
        draft.ReviewedByUserName = currentUser.UserName;

        await _drafts.SaveChangesAsync(ct);

        var dto = await _drafts.GetByIdAsync(draftId, ct);
        return new AppResult<PurchaseRequestDraftDetailDto>.Ok(dto!);
    }
}
