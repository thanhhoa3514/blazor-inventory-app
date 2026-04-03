using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public sealed class PurchaseRequestDraftRepository : IPurchaseRequestDraftRepository
{
    private readonly AppDbContext _db;

    public PurchaseRequestDraftRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PurchaseRequestDraftListDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.PurchaseRequestDrafts.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new PurchaseRequestDraftListDto(
                x.Id,
                x.DraftNo,
                x.Status,
                x.CreatedAtUtc,
                x.CreatedByUserName,
                x.Lines.Count))
            .ToListAsync(ct);

    public async Task<PurchaseRequestDraftDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.PurchaseRequestDrafts.AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .Include(x => x.Lines)
            .ThenInclude(x => x.Supplier)
            .Where(x => x.Id == id)
            .Select(x => new PurchaseRequestDraftDetailDto
            {
                Id = x.Id,
                DraftNo = x.DraftNo,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc,
                CreatedByUserName = x.CreatedByUserName,
                Note = x.Note,
                Lines = x.Lines
                    .OrderBy(l => l.Id)
                    .Select(l => new PurchaseRequestDraftLineDto(
                        l.Id,
                        l.ProductId,
                        l.Product != null ? l.Product.Sku : string.Empty,
                        l.Product != null ? l.Product.Name : string.Empty,
                        l.SupplierId,
                        l.Supplier != null ? l.Supplier.Name : null,
                        l.SuggestedQty,
                        l.RequestedQty))
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

    public async Task<PurchaseRequestDraft?> FindAsync(int id, CancellationToken ct = default)
        => await _db.PurchaseRequestDrafts
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PurchaseRequestDraftLine?> FindLineAsync(int draftId, int lineId, CancellationToken ct = default)
        => await _db.PurchaseRequestDraftLines.FirstOrDefaultAsync(
            x => x.PurchaseRequestDraftId == draftId && x.Id == lineId,
            ct);

    public async Task AddAsync(PurchaseRequestDraft entity, CancellationToken ct = default)
    {
        _db.PurchaseRequestDrafts.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public async Task RemoveLineAsync(PurchaseRequestDraftLine line, CancellationToken ct = default)
    {
        _db.PurchaseRequestDraftLines.Remove(line);
        await _db.SaveChangesAsync(ct);
    }
}
