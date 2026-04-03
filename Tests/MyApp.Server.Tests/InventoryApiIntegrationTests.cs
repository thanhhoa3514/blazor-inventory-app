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
    public async Task SupplierAndCustomerMasterData_AsAdmin_CanCreateUpdateAndDeactivate()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var supplier = await CreateSupplierAsync(client, "Warehouse Supplier");
        var customer = await CreateCustomerAsync(client, "Warehouse Customer");

        var updateSupplier = await client.PutAsJsonAsync($"/api/suppliers/{supplier.Id}", new UpdateSupplierRequest
        {
            Name = "Warehouse Supplier Updated",
            Description = "Updated by integration test",
            IsActive = true
        });
        updateSupplier.EnsureSuccessStatusCode();

        var updateCustomer = await client.PutAsJsonAsync($"/api/customers/{customer.Id}", new UpdateCustomerRequest
        {
            Name = "Warehouse Customer Updated",
            Description = "Updated by integration test",
            IsActive = true
        });
        updateCustomer.EnsureSuccessStatusCode();

        var deactivateSupplier = await client.DeleteAsync($"/api/suppliers/{supplier.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deactivateSupplier.StatusCode);

        var deactivateCustomer = await client.DeleteAsync($"/api/customers/{customer.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deactivateCustomer.StatusCode);

        var suppliers = await client.GetFromJsonAsync<List<SupplierDto>>("/api/suppliers");
        var customers = await client.GetFromJsonAsync<List<CustomerDto>>("/api/customers");

        Assert.NotNull(suppliers);
        Assert.NotNull(customers);

        var updatedSupplier = suppliers!.Single(x => x.Id == supplier.Id);
        var updatedCustomer = customers!.Single(x => x.Id == customer.Id);

        Assert.Equal("Warehouse Supplier Updated", updatedSupplier.Name);
        Assert.False(updatedSupplier.IsActive);
        Assert.Equal("Warehouse Customer Updated", updatedCustomer.Name);
        Assert.False(updatedCustomer.IsActive);

        var auditLogs = await client.GetFromJsonAsync<List<AuditLogDto>>("/api/audit-logs?entityType=Supplier&entityId=" + supplier.Id);
        Assert.NotNull(auditLogs);
        Assert.Contains(auditLogs!, x => x.Action == "Created" && x.ActorUserName == "admin");
        Assert.Contains(auditLogs!, x => x.Action == "Updated" && x.ActorUserName == "admin");
        Assert.Contains(auditLogs!, x => x.Action == "Deactivated" && x.ActorUserName == "admin");
    }

    [Fact]
    public async Task ReceiptIssueAndSummary_AsWarehouseStaff_Works()
    {
        using var adminClient = CreateClient();
        await LoginAsync(adminClient, "admin", AdminPassword);
        var supplier = await CreateSupplierAsync(adminClient, "Integration Supplier");
        var customer = await CreateCustomerAsync(adminClient, "Integration Customer");

        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var createReceipt = await client.PostAsJsonAsync("/api/receipts", new CreateStockReceiptRequest
        {
            SupplierId = supplier.Id,
            Lines =
            [
                new CreateStockReceiptLineRequest { ProductId = 1, Quantity = 5, UnitCost = 12m }
            ]
        });
        createReceipt.EnsureSuccessStatusCode();
        var createdReceipt = await createReceipt.Content.ReadFromJsonAsync<StockReceiptDetailDto>();
        Assert.NotNull(createdReceipt);
        Assert.Equal("staff", createdReceipt!.CreatedByUserName);

        var createIssue = await client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
            CustomerId = customer.Id,
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 3 }
            ]
        });
        createIssue.EnsureSuccessStatusCode();
        var createdIssue = await createIssue.Content.ReadFromJsonAsync<StockIssueDetailDto>();
        Assert.NotNull(createdIssue);
        Assert.Equal("staff", createdIssue!.CreatedByUserName);

        var summary = await client.GetFromJsonAsync<InventorySummaryDto>("/api/inventory/summary");
        Assert.NotNull(summary);
        Assert.True(summary!.TotalProducts >= 1);
        Assert.True(summary.TotalOnHandUnits >= 0);
    }

    [Fact]
    public async Task ProductStockCard_ReturnsReceiptIssueAndAdjustmentEntries_WithFiltering()
    {
        using var adminClient = CreateClient();
        await LoginAsync(adminClient, "admin", AdminPassword);
        var supplier = await CreateSupplierAsync(adminClient, "Stock Card Supplier");
        var customer = await CreateCustomerAsync(adminClient, "Stock Card Customer");

        using var client = CreateClient();
        await LoginAsync(client, "staff", StaffPassword);

        var receiptResponse = await client.PostAsJsonAsync("/api/receipts", new CreateStockReceiptRequest
        {
            SupplierId = supplier.Id,
            Lines =
            [
                new CreateStockReceiptLineRequest { ProductId = 1, Quantity = 2, UnitCost = 15m }
            ]
        });
        receiptResponse.EnsureSuccessStatusCode();

        var issueResponse = await client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
            CustomerId = customer.Id,
            Lines =
            [
                new CreateStockIssueLineRequest { ProductId = 1, Quantity = 1 }
            ]
        });
        issueResponse.EnsureSuccessStatusCode();

        var adjustmentResponse = await client.PostAsJsonAsync("/api/adjustments", new
        {
            reason = "Stock card test adjustment",
            lines = new[]
            {
                new
                {
                    productId = 1,
                    direction = "increase",
                    quantity = 3
                }
            }
        });
        adjustmentResponse.EnsureSuccessStatusCode();

        var stockCard = await client.GetFromJsonAsync<ProductStockCardDto>("/api/inventory/products/1/stock-card");
        Assert.NotNull(stockCard);
        Assert.Equal(1, stockCard!.ProductId);
        Assert.Contains(stockCard.Entries, x => x.MovementType == InventoryMovementTypes.Receipt);
        Assert.Contains(stockCard.Entries, x => x.MovementType == InventoryMovementTypes.Issue);
        Assert.Contains(stockCard.Entries, x => x.MovementType == InventoryMovementTypes.Adjustment);

        var issueOnly = await client.GetFromJsonAsync<ProductStockCardDto>("/api/inventory/products/1/stock-card?movementType=ISSUE");
        Assert.NotNull(issueOnly);
        Assert.NotEmpty(issueOnly!.Entries);
        Assert.All(issueOnly.Entries, x => Assert.Equal(InventoryMovementTypes.Issue, x.MovementType));

        var futureFrom = Uri.EscapeDataString(DateTime.UtcNow.AddDays(1).ToString("O"));
        var emptyFuture = await client.GetFromJsonAsync<ProductStockCardDto>($"/api/inventory/products/1/stock-card?fromUtc={futureFrom}");
        Assert.NotNull(emptyFuture);
        Assert.Empty(emptyFuture!.Entries);
    }

    [Fact]
    public async Task CreateIssue_AsViewer_ReturnsForbidden()
    {
        using var client = CreateClient();
        await LoginAsync(client, "viewer", ViewerPassword);

        var response = await client.PostAsJsonAsync("/api/issues", new CreateStockIssueRequest
        {
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
        Assert.Equal("staff", adjustment.CreatedByUserName);
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

    private static async Task<SupplierDto> CreateSupplierAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/suppliers", new CreateSupplierRequest
        {
            Name = name,
            Description = "Created by integration test"
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SupplierDto>())!;
    }

    private static async Task<CustomerDto> CreateCustomerAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest
        {
            Name = name,
            Description = "Created by integration test"
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CustomerDto>())!;
    }
}
