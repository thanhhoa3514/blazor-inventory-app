using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Commands;

public sealed class RemovePurchaseRequestDraftLineCommand
{
    private readonly IPurchaseRequestDraftRepository _drafts;

    public RemovePurchaseRequestDraftLineCommand(IPurchaseRequestDraftRepository drafts)
    {
        _drafts = drafts;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int draftId, int lineId, CancellationToken ct = default)
    {
        var draft = await _drafts.FindAsync(draftId, ct);
        if (draft is null)
            return new AppResult<Unit>.NotFound($"Draft {draftId} not found.");

        if (!string.Equals(draft.Status, PurchaseRequestDraftStatuses.Draft, StringComparison.OrdinalIgnoreCase))
            return new AppResult<Unit>.ValidationError("Only draft purchase requests can remove lines.");

        var line = await _drafts.FindLineAsync(draftId, lineId, ct);
        if (line is null)
            return new AppResult<Unit>.NotFound($"Draft line {lineId} not found.");

        await _drafts.RemoveLineAsync(line, ct);
        return new AppResult<Unit>.Ok(Unit.Value);
    }
}
