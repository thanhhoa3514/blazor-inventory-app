using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Queries;

public sealed class GetPurchaseRequestDraftByIdQuery
{
    private readonly IPurchaseRequestDraftRepository _drafts;

    public GetPurchaseRequestDraftByIdQuery(IPurchaseRequestDraftRepository drafts)
    {
        _drafts = drafts;
    }

    public async Task<AppResult<PurchaseRequestDraftDetailDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var item = await _drafts.GetByIdAsync(id, ct);
        return item is null
            ? new AppResult<PurchaseRequestDraftDetailDto>.NotFound($"Draft {id} not found.")
            : new AppResult<PurchaseRequestDraftDetailDto>.Ok(item);
    }
}
