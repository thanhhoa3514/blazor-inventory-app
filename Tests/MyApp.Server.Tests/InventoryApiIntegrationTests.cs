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
    public async Task CategoryAndProductLifecycle_AsAdmin_UsesSoftDelete()
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

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        Assert.NotNull(products);
        Assert.DoesNotContain(products!, x => x.Id == product.Id);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var softDeletedProduct = await db.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
            Assert.NotNull(softDeletedProduct);
            Assert.True(softDeletedProduct!.IsDeleted);
            Assert.False(softDeletedProduct.IsActive);

            var productAudit = await db.AuditLogs.AsNoTracking()
                .Where(x => x.EntityType == "Product" && x.EntityId == product.Id.ToString())
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            Assert.NotNull(productAudit);
            Assert.NotNull(productAudit!.BeforeJson);
            Assert.NotNull(productAudit.AfterJson);
            Assert.NotNull(productAudit.ChangedFieldsJson);
        }

        var deleteCategory = await client.DeleteAsync($"/api/categories/{category.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteCategory.StatusCode);

        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("/api/categories");
        Assert.NotNull(categories);
        Assert.DoesNotContain(categories!, x => x.Id == category.Id);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var softDeletedCategory = await db.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
            Assert.NotNull(softDeletedCategory);
            Assert.True(softDeletedCategory!.IsDeleted);

            var categoryAudit = await db.AuditLogs.AsNoTracking()
                .Where(x => x.EntityType == "Category" && x.EntityId == category.Id.ToString())
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            Assert.NotNull(categoryAudit);
            Assert.NotNull(categoryAudit!.BeforeJson);
            Assert.NotNull(categoryAudit.AfterJson);
            Assert.NotNull(categoryAudit.ChangedFieldsJson);
        }
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
        Assert.Contains(auditLogs!, x => x.Action == "Created" && x.ActorUserName == "admin" && x.AfterJson is not null);
        Assert.Contains(auditLogs!, x => x.Action == "Updated" && x.ActorUserName == "admin" && x.BeforeJson is not null && x.AfterJson is not null && x.ChangedFieldsJson is not null);
        Assert.Contains(auditLogs!, x => x.Action == "Deactivated" && x.ActorUserName == "admin" && x.BeforeJson is not null && x.AfterJson is not null && x.ChangedFieldsJson is not null);
    }

    [Fact]
    public async Task ProductAndCategoryDelete_AsAdmin_AreSoftDeletes()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var createCategory = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest
        {
            Name = "Soft Delete Category",
            Description = "Soft delete integration test"
        });
        createCategory.EnsureSuccessStatusCode();
        var category = await createCategory.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);

        var createProduct = await client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Sku = "SOFT-001",
            Name = "Soft Delete Product",
            CategoryId = category!.Id,
            ReorderLevel = 1
        });
        createProduct.EnsureSuccessStatusCode();
        var product = await createProduct.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);

        var deleteProduct = await client.DeleteAsync($"/api/products/{product!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteProduct.StatusCode);

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        Assert.NotNull(products);
        Assert.DoesNotContain(products!, x => x.Id == product.Id);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var deletedProduct = await db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == product.Id);
            Assert.NotNull(deletedProduct);
            Assert.True(deletedProduct!.IsDeleted);
            Assert.False(deletedProduct.IsActive);

            var productAudit = await db.AuditLogs.AsNoTracking()
                .Where(x => x.EntityType == "Product" && x.EntityId == product.Id.ToString())
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            Assert.NotNull(productAudit);
            Assert.Equal("Deleted", productAudit!.Action);
            Assert.NotNull(productAudit.BeforeJson);
            Assert.NotNull(productAudit.AfterJson);
            Assert.NotNull(productAudit.ChangedFieldsJson);
        }

        var deleteCategory = await client.DeleteAsync($"/api/categories/{category.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteCategory.StatusCode);

        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("/api/categories");
        Assert.NotNull(categories);
        Assert.DoesNotContain(categories!, x => x.Id == category.Id);
    }

    [Fact]
    public async Task RestoreProductAndCategory_AsAdmin_ReturnsEntitiesToActiveLists()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var createCategory = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest
        {
            Name = "Restore Category",
            Description = "Restore integration test"
        });
        createCategory.EnsureSuccessStatusCode();
        var category = await createCategory.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);

        var createProduct = await client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Sku = "REST-001",
            Name = "Restore Product",
            CategoryId = category!.Id,
            ReorderLevel = 2
        });
        createProduct.EnsureSuccessStatusCode();
        var product = await createProduct.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/products/{product!.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/categories/{category.Id}")).StatusCode);

        var restoreProduct = await client.PostAsync($"/api/products/{product.Id}/restore", null);
        restoreProduct.EnsureSuccessStatusCode();
        var restoreCategory = await client.PostAsync($"/api/categories/{category.Id}/restore", null);
        restoreCategory.EnsureSuccessStatusCode();

        var products = await client.GetFromJsonAsync<List<ProductDto>>("/api/products");
        var categories = await client.GetFromJsonAsync<List<CategoryDto>>("/api/categories");

        Assert.NotNull(products);
        Assert.NotNull(categories);
        Assert.Contains(products!, x => x.Id == product.Id && !x.IsDeleted);
        Assert.Contains(categories!, x => x.Id == category.Id && !x.IsDeleted);

        var restoredAudit = await client.GetFromJsonAsync<List<AuditLogDto>>($"/api/audit-logs?entityType=Product&entityId={product.Id}&action=Restored");
        Assert.NotNull(restoredAudit);
        Assert.NotEmpty(restoredAudit!);
    }

    [Fact]
    public async Task SoftDeleteAndRestoreSupplier_AsAdmin_PreservesOperationalViews()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var supplier = await CreateSupplierAsync(client, "Restore Supplier");

        var softDelete = await client.PostAsync($"/api/suppliers/{supplier.Id}/soft-delete", null);
        Assert.Equal(HttpStatusCode.NoContent, softDelete.StatusCode);

        var activeSuppliers = await client.GetFromJsonAsync<List<SupplierDto>>("/api/suppliers");
        var deletedSuppliers = await client.GetFromJsonAsync<List<SupplierDto>>("/api/suppliers/deleted");
        Assert.NotNull(activeSuppliers);
        Assert.NotNull(deletedSuppliers);
        Assert.DoesNotContain(activeSuppliers!, x => x.Id == supplier.Id);
        Assert.Contains(deletedSuppliers!, x => x.Id == supplier.Id && x.IsDeleted);

        var restore = await client.PostAsync($"/api/suppliers/{supplier.Id}/restore", null);
        Assert.Equal(HttpStatusCode.OK, restore.StatusCode);

        activeSuppliers = await client.GetFromJsonAsync<List<SupplierDto>>("/api/suppliers");
        Assert.NotNull(activeSuppliers);
        Assert.Contains(activeSuppliers!, x => x.Id == supplier.Id && !x.IsDeleted);
    }

    [Fact]
    public async Task AuditLog_ById_ReturnsSnapshots()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var categoryResponse = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest
        {
            Name = "Audit Detail Category",
            Description = "Before update"
        });
        categoryResponse.EnsureSuccessStatusCode();
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);

        var update = await client.PutAsJsonAsync($"/api/categories/{category!.Id}", new UpdateCategoryRequest
        {
            Name = "Audit Detail Category Updated",
            Description = "After update"
        });
        update.EnsureSuccessStatusCode();

        var logs = await client.GetFromJsonAsync<List<AuditLogDto>>($"/api/audit-logs?entityType=Category&entityId={category.Id}&action=Updated");
        Assert.NotNull(logs);
        var latest = Assert.Single(logs!);

        var detail = await client.GetFromJsonAsync<AuditLogDto>($"/api/audit-logs/{latest.Id}");
        Assert.NotNull(detail);
        Assert.NotNull(detail!.BeforeJson);
        Assert.NotNull(detail.AfterJson);
        Assert.NotNull(detail.ChangedFieldsJson);
        Assert.Contains("Before update", detail.BeforeJson!);
        Assert.Contains("After update", detail.AfterJson!);
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
    public async Task ReorderRecommendations_ReturnExpectedItemsAndFilters()
    {
        using var client = CreateClient();
        await LoginAsync(client, "admin", AdminPassword);

        var createCategory = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest
        {
            Name = "Reorder Category",
            Description = "Recommendation integration test"
        });
        createCategory.EnsureSuccessStatusCode();
        var category = await createCategory.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);

        var createProduct = await client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Sku = "REO-001",
            Name = "Reorder Product",
            CategoryId = category!.Id,
            ReorderLevel = 4,
            TargetStockLevel = 12
        });
        createProduct.EnsureSuccessStatusCode();
        var product = await createProduct.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);

        var recommendations = await client.GetFromJsonAsync<List<ReorderRecommendationDto>>("/api/inventory/reorder-recommendations");
        Assert.NotNull(recommendations);
        var item = recommendations!.Single(x => x.ProductId == product!.Id);
        Assert.Equal(0, item.OnHandQty);
        Assert.Equal(4, item.ReorderLevel);
        Assert.Equal(12, item.TargetStockLevel);
        Assert.Equal(12, item.SuggestedReorderQty);
        Assert.Equal(ReorderRecommendationPriorities.Critical, item.Priority);

        var byCategory = await client.GetFromJsonAsync<List<ReorderRecommendationDto>>($"/api/inventory/reorder-recommendations?categoryId={category.Id}");
        Assert.NotNull(byCategory);
        Assert.Contains(byCategory!, x => x.ProductId == product.Id);

        var byPriority = await client.GetFromJsonAsync<List<ReorderRecommendationDto>>($"/api/inventory/reorder-recommendations?priority={ReorderRecommendationPriorities.Critical}");
        Assert.NotNull(byPriority);
        Assert.Contains(byPriority!, x => x.ProductId == product.Id);

        await client.DeleteAsync($"/api/products/{product.Id}");
        recommendations = await client.GetFromJsonAsync<List<ReorderRecommendationDto>>("/api/inventory/reorder-recommendations");
        Assert.NotNull(recommendations);
        Assert.DoesNotContain(recommendations!, x => x.ProductId == product.Id);
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
