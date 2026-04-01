using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using Xunit;

namespace MyApp.Server.Tests;

public class InventoryApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string AdminPassword = "Admin!234";
    private const string StaffPassword = "Staff!234";
    private const string ViewerPassword = "Viewer!234";

    private readonly CustomWebApplicationFactory _factory;

    public InventoryApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSummary_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/inventory/summary");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CategoryAndProductLifecycle_AsAdmin_Works()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var createCategory = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest
        {
            Name = "Integration Cat",
            Description = "Created by integration test"
        });
        createCategory.EnsureSuccessStatusCode();

        var category = await createCategory.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);

        var createProduct = await client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Sku = "INT-001",
            Name = "Integration Product",
            CategoryId = category!.Id,
            ReorderLevel = 2
        });
        createProduct.EnsureSuccessStatusCode();

        var product = await createProduct.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);

        var getProduct = await client.GetAsync($"/api/products/{product!.Id}");
        getProduct.EnsureSuccessStatusCode();

        var deleteProduct = await client.DeleteAsync($"/api/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteProduct.StatusCode);

        var deleteCategory = await client.DeleteAsync($"/api/categories/{category.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteCategory.StatusCode);
    }

    [Fact]
    public async Task ReceiptIssueAndSummary_AsWarehouseStaff_Works()
    {
        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var createReceipt = await client.PostAsJsonAsync("/api/receipts", new CreateStockReceiptRequest
        {
            Supplier = "Test Supplier",
            Lines =
            [
                new CreateStockReceiptLineRequest { ProductId = 1, Quantity = 5, UnitCost = 12m }
            ]
        });
        createReceipt.EnsureSuccessStatusCode();

        var createIssue = await client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
            Customer = "Test Customer",
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 3 }
            ]
        });
        createIssue.EnsureSuccessStatusCode();

        var summary = await client.GetFromJsonAsync<InventorySummaryDto>("/api/inventory/summary");
        Assert.NotNull(summary);
        Assert.True(summary!.TotalProducts >= 1);
        Assert.True(summary.TotalOnHandUnits >= 0);
    }

    [Fact]
    public async Task CreateIssue_AsViewer_ReturnsForbidden()
    {
        using var client = CreateClient();
        await LoginAsync(client, "viewer", ViewerPassword);

        var response = await client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
            Customer = "Viewer Attempt",
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 1 }
            ]
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateAdjustment_AsWarehouseStaff_UpdatesProductAndWritesLedger()
    {
        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var startingProduct = await client.GetFromJsonAsync<ProductDto>("/api/products/1");
        Assert.NotNull(startingProduct);

        var response = await client.PostAsJsonAsync("/api/adjustments", new
        {
            reason = "Cycle count correction",
            note = "Found extra units during count",
            lines = new[]
            {
                new
                {
                    productId = 1,
                    direction = "increase",
                    quantity = 4
                }
            }
        });

        response.EnsureSuccessStatusCode();

        var product = await client.GetFromJsonAsync<ProductDto>("/api/products/1");
        Assert.NotNull(product);
        Assert.Equal(startingProduct!.OnHandQty + 4, product!.OnHandQty);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ledger = await db.InventoryLedgerEntries.AsNoTracking()
            .Where(x => x.ProductId == 1)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
        var adjustment = await db.StockAdjustments.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        Assert.NotNull(ledger);
        Assert.Equal("ADJUSTMENT", ledger!.MovementType);
        Assert.Equal(4, ledger.QuantityChange);
        Assert.Equal(startingProduct.OnHandQty + 4, ledger.RunningOnHandQty);

        Assert.NotNull(adjustment);
        Assert.Equal("Cycle count correction", adjustment!.Reason);
        Assert.Single(adjustment.Lines);
        Assert.Equal("increase", adjustment.Lines[0].Direction);
        Assert.Equal(4, adjustment.Lines[0].Quantity);
    }

    [Fact]
    public async Task CreateAdjustment_WithInsufficientStock_ReturnsBadRequest()
    {
        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var response = await client.PostAsJsonAsync("/api/adjustments", new
        {
            reason = "Damaged goods",
            lines = new[]
            {
                new
                {
                    productId = 1,
                    direction = "decrease",
                    quantity = 999
                }
            }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAdjustment_Decrease_AsWarehouseStaff_ReducesProduct()
    {
        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var startingProduct = await client.GetFromJsonAsync<ProductDto>("/api/products/1");
        Assert.NotNull(startingProduct);

        var response = await client.PostAsJsonAsync("/api/adjustments", new
        {
            reason = "Damaged goods",
            lines = new[]
            {
                new
                {
                    productId = 1,
                    direction = "decrease",
                    quantity = 2
                }
            }
        });

        response.EnsureSuccessStatusCode();

        var product = await client.GetFromJsonAsync<ProductDto>("/api/products/1");
        Assert.NotNull(product);
        Assert.Equal(startingProduct!.OnHandQty - 2, product!.OnHandQty);
    }

    [Fact]
    public async Task CreateAdjustment_WithoutReason_ReturnsBadRequest()
    {
        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var response = await client.PostAsJsonAsync("/api/adjustments", new
        {
            reason = "",
            lines = new[]
            {
                new
                {
                    productId = 1,
                    direction = "increase",
                    quantity = 1
                }
            }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAdjustment_AsViewer_ReturnsForbidden()
    {
        using var client = CreateClient();
        await LoginAsync(client, "viewer", ViewerPassword);

        var response = await client.PostAsJsonAsync("/api/adjustments", new
        {
            reason = "Viewer attempt",
            lines = new[]
            {
                new
                {
                    productId = 1,
                    direction = "increase",
                    quantity = 1
                }
            }
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AsWarehouseStaff_ReturnsForbidden()
    {
        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var response = await client.DeleteAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private HttpClient CreateClient()
        => _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });

    private static async Task LoginAsync(HttpClient client, string userNameOrEmail, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            UserNameOrEmail = userNameOrEmail,
            Password = password
        });

        response.EnsureSuccessStatusCode();
    }
}
