using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public interface IPurchaseRequestDraftRepository
{
    Task<IReadOnlyList<PurchaseRequestDraftListDto>> GetAllAsync(CancellationToken ct = default);
    Task<PurchaseRequestDraftDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PurchaseRequestDraft?> FindAsync(int id, CancellationToken ct = default);
    Task<PurchaseRequestDraftLine?> FindLineAsync(int draftId, int lineId, CancellationToken ct = default);
    Task AddAsync(PurchaseRequestDraft entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task RemoveLineAsync(PurchaseRequestDraftLine line, CancellationToken ct = default);
}
