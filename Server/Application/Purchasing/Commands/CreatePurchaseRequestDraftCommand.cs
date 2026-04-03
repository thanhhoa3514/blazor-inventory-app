using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Purchasing.Commands;

public sealed class CreatePurchaseRequestDraftCommand
{
    private readonly IPurchaseRequestDraftRepository _drafts;
    private readonly IProductRepository _products;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public CreatePurchaseRequestDraftCommand(
        IPurchaseRequestDraftRepository drafts,
        IProductRepository products,
        ICurrentUserAccessor currentUserAccessor)
    {
        _drafts = drafts;
        _products = products;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<AppResult<PurchaseRequestDraftDetailDto>> ExecuteAsync(
        CreatePurchaseRequestDraftRequest request,
        CancellationToken ct = default)
    {
        if (request.Lines.Count == 0)
            return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError("Draft must contain at least one line.");

        var currentUser = _currentUserAccessor.GetRequiredCurrentUser();
        var now = DateTime.UtcNow;
        var draftNo = $"PRD-{now:yyyyMMddHHmmssfff}";

        var draft = new PurchaseRequestDraft
        {
            DraftNo = draftNo,
            Status = PurchaseRequestDraftStatuses.Draft,
            CreatedAtUtc = now,
            CreatedByUserId = currentUser.UserId,
            CreatedByUserName = currentUser.UserName,
            Note = request.Note?.Trim()
        };

        foreach (var line in request.Lines)
        {
            var product = await _products.FindActiveAsync(line.ProductId, ct);
            if (product is null)
                return new AppResult<PurchaseRequestDraftDetailDto>.ValidationError($"Product with ID {line.ProductId} does not exist or is inactive.");

            draft.Lines.Add(new PurchaseRequestDraftLine
            {
                ProductId = product.Id,
                SupplierId = null,
                SuggestedQty = line.SuggestedQty,
                RequestedQty = line.RequestedQty
            });
        }

        await _drafts.AddAsync(draft, ct);
        var dto = await _drafts.GetByIdAsync(draft.Id, ct);
        return new AppResult<PurchaseRequestDraftDetailDto>.Ok(dto!);
    }
}
