using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Products.Commands;
using MyApp.Server.Application.Products.Queries;
using MyApp.Server.Data;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;
using Xunit;

namespace MyApp.Server.Tests;

public class ProductUseCaseTests
{
    // ── category existence check ────────────────────────────────────────────

    [Fact]
    public async Task CreateProduct_WithMissingCategory_ReturnsValidationError()
    {
        await using var db = await CreateContextAsync();
        var repo = new ProductRepository(db);
        var cmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());

        var result = await cmd.ExecuteAsync(new CreateProductRequest
        {
            Sku = "X-001",
            Name = "Ghost Product",
            CategoryId = 9999,
            ReorderLevel = 5
        });

        Assert.IsType<AppResult<ProductDto>.ValidationError>(result);
    }

    [Fact]
    public async Task UpdateProduct_WithMissingCategory_ReturnsValidationError()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var createCmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());
        var updateCmd = new UpdateProductCommand(repo, new NoOpAuditLogWriter());

        var created = (AppResult<ProductDto>.Ok)await createCmd.ExecuteAsync(new CreateProductRequest
        {
            Sku = "P-001",
            Name = "Widget",
            CategoryId = catId,
            ReorderLevel = 5
        });

        var result = await updateCmd.ExecuteAsync(created.Value.Id, new UpdateProductRequest
        {
            Sku = "P-001",
            Name = "Widget",
            CategoryId = 9999,
            ReorderLevel = 5,
            IsActive = true
        });

        Assert.IsType<AppResult<ProductDto>.ValidationError>(result);
    }

    // ── duplicate SKU ───────────────────────────────────────────────────────

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ReturnsConflict()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var cmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());

        await cmd.ExecuteAsync(new CreateProductRequest { Sku = "SKU-001", Name = "First", CategoryId = catId, ReorderLevel = 5 });
        var result = await cmd.ExecuteAsync(new CreateProductRequest { Sku = "SKU-001", Name = "Second", CategoryId = catId, ReorderLevel = 5 });

        Assert.IsType<AppResult<ProductDto>.Conflict>(result);
    }

    [Fact]
    public async Task CreateProduct_NormalizesSku_ToUpperCase()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var cmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());

        var result = await cmd.ExecuteAsync(new CreateProductRequest
        {
            Sku = "  abc-001  ",
            Name = "Widget",
            CategoryId = catId,
            ReorderLevel = 5
        });

        var ok = Assert.IsType<AppResult<ProductDto>.Ok>(result);
        Assert.Equal("ABC-001", ok.Value.Sku);
    }

    [Fact]
    public async Task UpdateProduct_WithDuplicateSku_ReturnsConflict()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var createCmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());
        var updateCmd = new UpdateProductCommand(repo, new NoOpAuditLogWriter());

        await createCmd.ExecuteAsync(new CreateProductRequest { Sku = "TAKEN", Name = "Taken", CategoryId = catId, ReorderLevel = 5 });
        var second = (AppResult<ProductDto>.Ok)await createCmd.ExecuteAsync(new CreateProductRequest { Sku = "MINE", Name = "Mine", CategoryId = catId, ReorderLevel = 5 });

        var result = await updateCmd.ExecuteAsync(second.Value.Id, new UpdateProductRequest
        {
            Sku = "TAKEN",
            Name = "Mine",
            CategoryId = catId,
            ReorderLevel = 5,
            IsActive = true
        });

        Assert.IsType<AppResult<ProductDto>.Conflict>(result);
    }

    // ── soft delete behavior ───────────────────────────────────────────────

    [Fact]
    public async Task DeleteProduct_WithReceiptHistory_SoftDeletesAndKeepsRow()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var createCmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());
        var deleteCmd = new DeleteProductCommand(repo, new NoOpAuditLogWriter(), new StaticCurrentUserAccessor(), NullLogger<DeleteProductCommand>.Instance);

        var product = (AppResult<ProductDto>.Ok)await createCmd.ExecuteAsync(new CreateProductRequest
        {
            Sku = "DEL-001",
            Name = "Deletable",
            CategoryId = catId,
            ReorderLevel = 5
        });

        var receipt = new StockReceipt { DocumentNo = "REC-001", CreatedByUserName = "seed.user", ReceivedAtUtc = DateTime.UtcNow };
        db.StockReceipts.Add(receipt);
        await db.SaveChangesAsync();

        db.StockReceiptLines.Add(new StockReceiptLine
        {
            StockReceiptId = receipt.Id,
            ProductId = product.Value.Id,
            Quantity = 10,
            UnitCost = 5m,
            LineTotal = 50m
        });
        await db.SaveChangesAsync();

        var result = await deleteCmd.ExecuteAsync(product.Value.Id);

        Assert.IsType<AppResult<Unit>.Ok>(result);

        var deletedEntity = await db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == product.Value.Id);
        Assert.NotNull(deletedEntity);
        Assert.True(deletedEntity!.IsDeleted);
        Assert.False(deletedEntity.IsActive);
        Assert.Equal("test.user", deletedEntity.DeletedByUserName);
    }

    [Fact]
    public async Task DeleteProduct_NoHistory_SoftDeletesRow()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var createCmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());
        var deleteCmd = new DeleteProductCommand(repo, new NoOpAuditLogWriter(), new StaticCurrentUserAccessor(), NullLogger<DeleteProductCommand>.Instance);

        var product = (AppResult<ProductDto>.Ok)await createCmd.ExecuteAsync(new CreateProductRequest
        {
            Sku = "DEL-002",
            Name = "Clean",
            CategoryId = catId,
            ReorderLevel = 5
        });

        var result = await deleteCmd.ExecuteAsync(product.Value.Id);

        Assert.IsType<AppResult<Unit>.Ok>(result);
        var deleted = await db.Products.AsNoTracking().FirstAsync(x => x.Id == product.Value.Id);
        Assert.True(deleted.IsDeleted);
        Assert.False(deleted.IsActive);
        Assert.Equal("test.user", deleted.DeletedByUserName);

        var deletedEntity = await db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == product.Value.Id);
        Assert.NotNull(deletedEntity);
        Assert.True(deletedEntity!.IsDeleted);
        Assert.False(deletedEntity.IsActive);
        Assert.Equal("test.user", deletedEntity.DeletedByUserName);
    }

    [Fact]
    public async Task DeleteProduct_NotFound_ReturnsNotFound()
    {
        await using var db = await CreateContextAsync();
        var repo = new ProductRepository(db);
        var deleteCmd = new DeleteProductCommand(repo, new NoOpAuditLogWriter(), new StaticCurrentUserAccessor(), NullLogger<DeleteProductCommand>.Instance);

        var result = await deleteCmd.ExecuteAsync(9999);

        Assert.IsType<AppResult<Unit>.NotFound>(result);
    }

    // ── query ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProductById_NotFound_ReturnsNotFound()
    {
        await using var db = await CreateContextAsync();
        var repo = new ProductRepository(db);
        var query = new GetProductByIdQuery(repo);

        var result = await query.ExecuteAsync(9999);

        Assert.IsType<AppResult<ProductDto>.NotFound>(result);
    }

    [Fact]
    public async Task CreateProduct_SetsDefaultFields()
    {
        await using var db = await CreateContextAsync();
        var (repo, catId) = await CreateRepoWithCategoryAsync(db);
        var cmd = new CreateProductCommand(repo, new NoOpAuditLogWriter());

        var result = await cmd.ExecuteAsync(new CreateProductRequest
        {
            Sku = "DEF-001",
            Name = "Defaults",
            CategoryId = catId,
            ReorderLevel = 3
        });

        var ok = Assert.IsType<AppResult<ProductDto>.Ok>(result);
        Assert.Equal(0, ok.Value.OnHandQty);
        Assert.Equal(0m, ok.Value.AverageCost);
        Assert.True(ok.Value.IsActive);
    }

    // ── helpers ─────────────────────────────────────────────────────────────

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

    private static async Task<(ProductRepository repo, int categoryId)> CreateRepoWithCategoryAsync(AppDbContext db)
    {
        var cat = new Category { Name = "Test Category", CreatedAtUtc = DateTime.UtcNow };
        db.Categories.Add(cat);
        await db.SaveChangesAsync();

        return (new ProductRepository(db), cat.Id);
    }
}
