using System.Net;
using System.Net.Http.Json;
using MyApp.Shared.Contracts;
using Xunit;

namespace MyApp.Server.Tests;

public class InventoryApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InventoryApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CategoryAndProductLifecycle_Works()
    {
        var createCategory = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest
        {
            Name = "Integration Cat",
            Description = "Created by integration test"
        });
        createCategory.EnsureSuccessStatusCode();

        var category = await createCategory.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);

        var createProduct = await _client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Sku = "INT-001",
            Name = "Integration Product",
            CategoryId = category!.Id,
            ReorderLevel = 2
        });
        createProduct.EnsureSuccessStatusCode();

        var product = await createProduct.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);

        var getProduct = await _client.GetAsync($"/api/products/{product!.Id}");
        getProduct.EnsureSuccessStatusCode();

        var deleteProduct = await _client.DeleteAsync($"/api/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteProduct.StatusCode);

        var deleteCategory = await _client.DeleteAsync($"/api/categories/{category.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteCategory.StatusCode);
    }

    [Fact]
    public async Task ReceiptIssueAndSummary_FlowWorks()
    {
        var createReceipt = await _client.PostAsJsonAsync("/api/receipts", new CreateStockReceiptRequest
        {
            Supplier = "Test Supplier",
            Lines =
            [
                new CreateStockReceiptLineRequest { ProductId = 1, Quantity = 5, UnitCost = 12m }
            ]
        });
        createReceipt.EnsureSuccessStatusCode();

        var createIssue = await _client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
            Customer = "Test Customer",
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 3 }
            ]
        });
        createIssue.EnsureSuccessStatusCode();

        var summary = await _client.GetFromJsonAsync<InventorySummaryDto>("/api/inventory/summary");
        Assert.NotNull(summary);
        Assert.True(summary!.TotalProducts >= 1);
        Assert.True(summary.TotalOnHandUnits >= 0);
    }

    [Fact]
    public async Task IssueMoreThanOnHand_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
            Customer = "Overflow",
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 9999 }
            ]
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
