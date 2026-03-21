using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyApp.Server.Data;
using MyApp.Server.Services;
using MyApp.Shared.Contracts;
using Xunit;

namespace MyApp.Server.Tests;

public class InventoryServiceTests
{
    [Fact]
    public async Task CreateReceipt_UpdatesMovingAverageCost()
    {
        await using var db = await CreateContextAsync();
        var service = new InventoryService(db, NullLogger<InventoryService>.Instance);

        var result = await service.CreateReceiptAsync(new CreateStockReceiptRequest
        {
            Supplier = "Supplier A",
            Lines =
            [
                new CreateStockReceiptLineRequest { ProductId = 1, Quantity = 10, UnitCost = 20m }
            ]
        });

        Assert.NotEqual(0, result.Id);
        var product = await db.Products.AsNoTracking().FirstAsync(x => x.Id == 1);
        Assert.Equal(30, product.OnHandQty);
        Assert.Equal(13.33m, product.AverageCost);
    }

    [Fact]
    public async Task CreateIssue_ReducesStock_AndUsesAverageCost()
    {
        await using var db = await CreateContextAsync();
        var service = new InventoryService(db, NullLogger<InventoryService>.Instance);

        var issue = await service.CreateIssueAsync(new CreateStockIssueRequest
        {
            Customer = "Customer A",
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 5 }
            ]
        });

        var product = await db.Products.AsNoTracking().FirstAsync(x => x.Id == 1);
        Assert.Equal(15, product.OnHandQty);
        Assert.Equal(10m, issue.Lines.Single().UnitCost);
        Assert.Equal(50m, issue.TotalAmount);
    }

    [Fact]
    public async Task CreateIssue_WithInsufficientStock_ThrowsValidation()
    {
        await using var db = await CreateContextAsync();
        var service = new InventoryService(db, NullLogger<InventoryService>.Instance);

        await Assert.ThrowsAsync<InventoryValidationException>(() => service.CreateIssueAsync(new CreateStockIssueRequest
        {
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 9999 }
            ]
        }));
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

    private static async Task<AppDbContext> CreateContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return db;
    }
}

