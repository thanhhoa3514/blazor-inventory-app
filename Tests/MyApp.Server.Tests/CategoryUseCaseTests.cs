using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyApp.Server.Application.Categories.Commands;
using MyApp.Server.Application.Categories.Queries;
using MyApp.Server.Application.Common;
using MyApp.Server.Data;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;
using Xunit;

namespace MyApp.Server.Tests;

public class CategoryUseCaseTests
{
    // ── duplicate name ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCategory_WithDuplicateName_ReturnsConflict()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var cmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());

        await cmd.ExecuteAsync(new CreateCategoryRequest { Name = "Electronics" });
        var result = await cmd.ExecuteAsync(new CreateCategoryRequest { Name = "Electronics" });

        Assert.IsType<AppResult<CategoryDto>.Conflict>(result);
    }

    [Fact]
    public async Task CreateCategory_TrimsName()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var cmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());

        var result = await cmd.ExecuteAsync(new CreateCategoryRequest { Name = "  Furniture  " });

        var ok = Assert.IsType<AppResult<CategoryDto>.Ok>(result);
        Assert.Equal("Furniture", ok.Value.Name);
    }

    [Fact]
    public async Task UpdateCategory_WithDuplicateName_ReturnsConflict()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var createCmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());
        var updateCmd = new UpdateCategoryCommand(repo, new NoOpAuditLogWriter());

        var first = (AppResult<CategoryDto>.Ok)await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "Furniture" });
        await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "Hardware" });

        var result = await updateCmd.ExecuteAsync(first.Value.Id, new UpdateCategoryRequest { Name = "Hardware" });

        Assert.IsType<AppResult<CategoryDto>.Conflict>(result);
    }

    [Fact]
    public async Task UpdateCategory_SameNameOnSelf_Succeeds()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var createCmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());
        var updateCmd = new UpdateCategoryCommand(repo, new NoOpAuditLogWriter());

        var first = (AppResult<CategoryDto>.Ok)await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "Furniture" });

        var result = await updateCmd.ExecuteAsync(first.Value.Id, new UpdateCategoryRequest { Name = "Furniture", Description = "Updated" });

        var ok = Assert.IsType<AppResult<CategoryDto>.Ok>(result);
        Assert.Equal("Updated", ok.Value.Description);
    }

    [Fact]
    public async Task UpdateCategory_NotFound_ReturnsNotFound()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var updateCmd = new UpdateCategoryCommand(repo, new NoOpAuditLogWriter());

        var result = await updateCmd.ExecuteAsync(9999, new UpdateCategoryRequest { Name = "Ghost" });

        Assert.IsType<AppResult<CategoryDto>.NotFound>(result);
    }

    // ── delete blocked by products ──────────────────────────────────────────

    [Fact]
    public async Task DeleteCategory_WithExistingProducts_ReturnsConflict()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var createCmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());
        var deleteCmd = new DeleteCategoryCommand(repo, new NoOpAuditLogWriter(), new StaticCurrentUserAccessor());

        var catResult = (AppResult<CategoryDto>.Ok)await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "Busy" });
        int catId = catResult.Value.Id;

        db.Products.Add(new Product
        {
            Sku = "P-001",
            Name = "Some Product",
            CategoryId = catId,
            ReorderLevel = 1,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var result = await deleteCmd.ExecuteAsync(catId);

        Assert.IsType<AppResult<Unit>.Conflict>(result);
    }

    [Fact]
    public async Task DeleteCategory_Empty_SoftDeletesRow()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var createCmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());
        var deleteCmd = new DeleteCategoryCommand(repo, new NoOpAuditLogWriter(), new StaticCurrentUserAccessor());

        var catResult = (AppResult<CategoryDto>.Ok)await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "Empty" });

        var result = await deleteCmd.ExecuteAsync(catResult.Value.Id);

        Assert.IsType<AppResult<Unit>.Ok>(result);
        var deleted = await db.Categories.AsNoTracking().FirstAsync(x => x.Id == catResult.Value.Id);
        Assert.True(deleted.IsDeleted);
        Assert.Equal("test.user", deleted.DeletedByUserName);

        var deletedEntity = await db.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == catResult.Value.Id);
        Assert.NotNull(deletedEntity);
        Assert.True(deletedEntity!.IsDeleted);
        Assert.Equal("test.user", deletedEntity.DeletedByUserName);
    }

    [Fact]
    public async Task DeleteCategory_NotFound_ReturnsNotFound()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var deleteCmd = new DeleteCategoryCommand(repo, new NoOpAuditLogWriter(), new StaticCurrentUserAccessor());

        var result = await deleteCmd.ExecuteAsync(9999);

        Assert.IsType<AppResult<Unit>.NotFound>(result);
    }

    // ── query ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllCategories_ReturnsSortedList()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var createCmd = new CreateCategoryCommand(repo, new NoOpAuditLogWriter());
        var query = new GetAllCategoriesQuery(repo);

        // Seeded DB already has "Electronics" and "Office Supplies".
        // Add two more names that sort to the outer edges.
        await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "Zeta Category" });
        await createCmd.ExecuteAsync(new CreateCategoryRequest { Name = "AAA Category" });

        var list = await query.ExecuteAsync();

        // Verify sort order: first entry should be "AAA Category", last should be "Zeta Category".
        Assert.Equal("AAA Category", list[0].Name);
        Assert.Equal("Zeta Category", list[^1].Name);
    }

    [Fact]
    public async Task GetCategoryById_NotFound_ReturnsNotFound()
    {
        await using var db = await CreateContextAsync();
        var repo = new CategoryRepository(db);
        var query = new GetCategoryByIdQuery(repo);

        var result = await query.ExecuteAsync(9999);

        Assert.IsType<AppResult<CategoryDto>.NotFound>(result);
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
}
