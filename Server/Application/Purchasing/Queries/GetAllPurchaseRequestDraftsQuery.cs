using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Purchasing.Queries;

public sealed class GetAllPurchaseRequestDraftsQuery
{
    private readonly IPurchaseRequestDraftRepository _drafts;

    public GetAllPurchaseRequestDraftsQuery(IPurchaseRequestDraftRepository drafts)
    {
        _drafts = drafts;
    }

    public Task<IReadOnlyList<PurchaseRequestDraftListDto>> ExecuteAsync(CancellationToken ct = default)
        => _drafts.GetAllAsync(ct);
}
