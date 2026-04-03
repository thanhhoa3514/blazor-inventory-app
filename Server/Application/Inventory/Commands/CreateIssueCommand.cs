using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Inventory.Commands;

public sealed class CreateIssueCommand
{
    private readonly IInventoryUnitOfWork _uow;
    private readonly IProductRepository _products;
    private readonly ICustomerRepository _customers;
    private readonly IStockIssueRepository _issues;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<CreateIssueCommand> _logger;

    public CreateIssueCommand(
        IInventoryUnitOfWork uow,
        IProductRepository products,
        ICustomerRepository customers,
        IStockIssueRepository issues,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<CreateIssueCommand> logger)
    {
        _uow = uow;
        _products = products;
        _customers = customers;
        _issues = issues;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    public async Task<AppResult<StockIssueDetailDto>> ExecuteAsync(
        CreateStockIssueRequest request,
        CancellationToken ct = default)
    {
        var validationError = ValidateRequest(request);
        if (validationError is not null)
            return new AppResult<StockIssueDetailDto>.ValidationError(validationError);

        var now = DateTime.UtcNow;
        var documentNo = $"ISS-{now:yyyyMMddHHmmssfff}";

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var currentUser = _currentUserAccessor.GetRequiredCurrentUser();

            Customer? customer = null;
            if (request.CustomerId.HasValue)
            {
                customer = await _customers.FindActiveAsync(request.CustomerId.Value, ct);
                if (customer is null)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockIssueDetailDto>.ValidationError(
                        $"Customer with ID {request.CustomerId.Value} does not exist or is inactive.");
                }
            }

            var issue = new StockIssue
            {
                DocumentNo = documentNo,
                CustomerId = customer?.Id,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                Note = request.Note?.Trim(),
                IssuedAtUtc = now
            };

            decimal total = 0m;
            foreach (var line in request.Lines)
            {
                var product = await _products.FindActiveAsync(line.ProductId, ct);
                if (product is null)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockIssueDetailDto>.ValidationError(
                        $"Product with ID {line.ProductId} does not exist or is inactive.");
                }

                if (product.OnHandQty < line.Quantity)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockIssueDetailDto>.ValidationError(
                        $"Insufficient stock for {product.Sku}. On hand {product.OnHandQty}, requested {line.Quantity}.");
                }

                var unitCost = product.AverageCost;
                var lineTotal = Math.Round(line.Quantity * unitCost, 2, MidpointRounding.AwayFromZero);

                product.OnHandQty -= line.Quantity;
                product.LastUpdatedUtc = now;

                issue.Lines.Add(new StockIssueLine
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitCost = unitCost,
                    LineTotal = lineTotal
                });

                _uow.AddLedgerEntry(new InventoryLedgerEntry
                {
                    ProductId = product.Id,
                    MovementType = "ISSUE",
                    ReferenceNo = documentNo,
                    OccurredAtUtc = now,
                    QuantityChange = -line.Quantity,
                    UnitCost = unitCost,
                    ValueChange = -lineTotal,
                    RunningOnHandQty = product.OnHandQty,
                    RunningAverageCost = product.AverageCost
                });

                total += lineTotal;
            }

            issue.TotalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            _uow.AddIssue(issue);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Created issue {DocumentNo} with {LineCount} lines and total {TotalAmount}.",
                issue.DocumentNo, issue.Lines.Count, issue.TotalAmount);

            var dto = await _issues.GetByIdAsync(issue.Id, ct)
                ?? throw new InvalidOperationException("Issue created but could not be loaded.");
            return new AppResult<StockIssueDetailDto>.Ok(dto);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            await _uow.RollbackAsync(ct);
            _logger.LogError(ex, "Failed to create stock issue.");
            throw;
        }
    }

    private static string? ValidateRequest(CreateStockIssueRequest request)
    {
        if (request.Lines.Count == 0)
            return "Issue must contain at least one line.";
        if (request.Lines.Any(x => x.Quantity <= 0))
            return "Issue quantities must be greater than zero.";
        return null;
    }
}
