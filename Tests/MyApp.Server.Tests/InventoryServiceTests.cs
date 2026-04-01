using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Inventory.Commands;
using MyApp.Server.Data;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using Xunit;

namespace MyApp.Server.Tests;

public class InventoryCommandTests
{
    [Fact]
    public async Task CreateReceipt_UpdatesMovingAverageCost()
    {
        await using var db = await CreateContextAsync();
        var cmd = BuildReceiptCommand(db);

        var result = await cmd.ExecuteAsync(new CreateStockReceiptRequest
        {
            Supplier = "Supplier A",
            Lines =
            [
                new CreateStockReceiptLineRequest { ProductId = 1, Quantity = 10, UnitCost = 20m }
            ]
        });

        var ok = Assert.IsType<AppResult<StockReceiptDetailDto>.Ok>(result);
        Assert.NotEqual(0, ok.Value.Id);
        var product = await db.Products.AsNoTracking().FirstAsync(x => x.Id == 1);
        Assert.Equal(30, product.OnHandQty);
        Assert.Equal(13.33m, product.AverageCost);
    }

    [Fact]
    public async Task CreateIssue_ReducesStock_AndUsesAverageCost()
    {
        await using var db = await CreateContextAsync();
        var cmd = BuildIssueCommand(db);

        var result = await cmd.ExecuteAsync(new CreateStockIssueRequest
        {
            Customer = "Customer A",
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 5 }
            ]
        });

        var ok = Assert.IsType<AppResult<StockIssueDetailDto>.Ok>(result);
        var product = await db.Products.AsNoTracking().FirstAsync(x => x.Id == 1);
        Assert.Equal(15, product.OnHandQty);
        Assert.Equal(10m, ok.Value.Lines.Single().UnitCost);
        Assert.Equal(50m, ok.Value.TotalAmount);
    }

    [Fact]
    public async Task CreateIssue_WithInsufficientStock_ReturnsValidationError()
    {
        await using var db = await CreateContextAsync();
        var cmd = BuildIssueCommand(db);

        var result = await cmd.ExecuteAsync(new CreateStockIssueRequest
        {
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 9999 }
            ]
        });

        Assert.IsType<AppResult<StockIssueDetailDto>.ValidationError>(result);
    }

    [Fact]
    public async Task CreateAdjustment_Increase_UpdatesStock()
    {
        await using var db = await CreateContextAsync();
        var cmd = BuildAdjustmentCommand(db);

        var result = await cmd.ExecuteAsync(new CreateStockAdjustmentRequest
        {
            Reason = "Inventory count correction",
            Lines =
            [
                new CreateStockAdjustmentLineRequest { ProductId = 1, Quantity = 5, Direction = "increase" }
            ]
        });

        var ok = Assert.IsType<AppResult<StockAdjustmentDetailDto>.Ok>(result);
        var product = await db.Products.AsNoTracking().FirstAsync(x => x.Id == 1);
        Assert.Equal(25, product.OnHandQty);
    }

    [Fact]
    public async Task CreateAdjustment_WithInsufficientStock_ReturnsValidationError()
    {
        await using var db = await CreateContextAsync();
        var cmd = BuildAdjustmentCommand(db);

        var result = await cmd.ExecuteAsync(new CreateStockAdjustmentRequest
        {
            Reason = "Test",
            Lines =
            [
                new CreateStockAdjustmentLineRequest { ProductId = 1, Quantity = 9999, Direction = "decrease" }
            ]
        });

        Assert.IsType<AppResult<StockAdjustmentDetailDto>.ValidationError>(result);
    }

    [Fact]
    public async Task CreateAdjustment_WithoutReason_ReturnsValidationError()
    {
        await using var db = await CreateContextAsync();
        var cmd = BuildAdjustmentCommand(db);

        var result = await cmd.ExecuteAsync(new CreateStockAdjustmentRequest
        {
            Reason = "   ",
            Lines =
            [
                new CreateStockAdjustmentLineRequest { ProductId = 1, Quantity = 1, Direction = "increase" }
            ]
        });

        Assert.IsType<AppResult<StockAdjustmentDetailDto>.ValidationError>(result);
    }

    [Fact]
    public async Task DuplicateSku_Insert_ThrowsDbUpdateException()
    {
        await using var db = await CreateContextAsync();

        db.Products.Add(new MyApp.Shared.Domain.Product
        {
            Sku = "ELEC-001",
            Name = "Duplicate",
            CategoryId = 1,
            ReorderLevel = 1,
            IsActive = true,
            LastUpdatedUtc = DateTime.UtcNow,
            CreatedAtUtc = DateTime.UtcNow
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    private static CreateReceiptCommand BuildReceiptCommand(AppDbContext db)
        => new(new InventoryUnitOfWork(db), new ProductRepository(db),
               new StockReceiptRepository(db), NullLogger<CreateReceiptCommand>.Instance);

    private static CreateIssueCommand BuildIssueCommand(AppDbContext db)
        => new(new InventoryUnitOfWork(db), new ProductRepository(db),
               new StockIssueRepository(db), NullLogger<CreateIssueCommand>.Instance);

    private static CreateAdjustmentCommand BuildAdjustmentCommand(AppDbContext db)
        => new(new InventoryUnitOfWork(db), new ProductRepository(db),
               new StockAdjustmentRepository(db), NullLogger<CreateAdjustmentCommand>.Instance);

    private static async Task<AppDbContext> CreateContextAsync()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return db;
    }
}
