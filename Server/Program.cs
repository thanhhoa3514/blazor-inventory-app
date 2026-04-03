using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Application.Audit.Queries;
using MyApp.Server.Application.Categories.Commands;
using MyApp.Server.Application.Categories.Queries;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Customers.Commands;
using MyApp.Server.Application.Customers.Queries;
using MyApp.Server.Application.Inventory.Commands;
using MyApp.Server.Application.Inventory.Queries;
using MyApp.Server.Application.Purchasing.Commands;
using MyApp.Server.Application.Purchasing.Queries;
using MyApp.Server.Application.Products.Commands;
using MyApp.Server.Application.Products.Queries;
using MyApp.Server.Application.Suppliers.Commands;
using MyApp.Server.Application.Suppliers.Queries;
using MyApp.Server.Auth;
using MyApp.Server.Data;
using MyApp.Server.Persistence.Auditing;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql =>
        {
            sql.CommandTimeout(120);
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        });
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (IsApiRequest(context.Request.Path))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (IsApiRequest(context.Request.Path))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.ReadAccess, policy =>
        policy.RequireRole(AppRoles.Admin, AppRoles.WarehouseStaff, AppRoles.Viewer));

    options.AddPolicy(AppPolicies.WarehouseOperations, policy =>
        policy.RequireRole(AppRoles.Admin, AppRoles.WarehouseStaff));

    options.AddPolicy(AppPolicies.AdminOnly, policy =>
        policy.RequireRole(AppRoles.Admin));
});

builder.Services.Configure<AuthSeedOptions>(builder.Configuration.GetSection("Auth:Seed"));

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IStockReceiptRepository, StockReceiptRepository>();
builder.Services.AddScoped<IStockIssueRepository, StockIssueRepository>();
builder.Services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
builder.Services.AddScoped<IInventoryReadRepository, InventoryReadRepository>();
builder.Services.AddScoped<IReorderReadRepository, ReorderReadRepository>();
builder.Services.AddScoped<IPurchaseRequestDraftRepository, PurchaseRequestDraftRepository>();
builder.Services.AddScoped<IInventoryUnitOfWork, InventoryUnitOfWork>();
builder.Services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
builder.Services.AddScoped<IAuditLogWriter, AuditLogWriter>();

// Category use cases
builder.Services.AddScoped<GetAllCategoriesQuery>();
builder.Services.AddScoped<GetDeletedCategoriesQuery>();
builder.Services.AddScoped<GetCategoryByIdQuery>();
builder.Services.AddScoped<CreateCategoryCommand>();
builder.Services.AddScoped<UpdateCategoryCommand>();
builder.Services.AddScoped<DeleteCategoryCommand>();
builder.Services.AddScoped<RestoreCategoryCommand>();

// Supplier use cases
builder.Services.AddScoped<GetAllSuppliersQuery>();
builder.Services.AddScoped<GetDeletedSuppliersQuery>();
builder.Services.AddScoped<GetSupplierByIdQuery>();
builder.Services.AddScoped<CreateSupplierCommand>();
builder.Services.AddScoped<UpdateSupplierCommand>();
builder.Services.AddScoped<DeactivateSupplierCommand>();
builder.Services.AddScoped<SoftDeleteSupplierCommand>();
builder.Services.AddScoped<RestoreSupplierCommand>();

// Customer use cases
builder.Services.AddScoped<GetAllCustomersQuery>();
builder.Services.AddScoped<GetDeletedCustomersQuery>();
builder.Services.AddScoped<GetCustomerByIdQuery>();
builder.Services.AddScoped<CreateCustomerCommand>();
builder.Services.AddScoped<UpdateCustomerCommand>();
builder.Services.AddScoped<DeactivateCustomerCommand>();
builder.Services.AddScoped<SoftDeleteCustomerCommand>();
builder.Services.AddScoped<RestoreCustomerCommand>();

// Product use cases
builder.Services.AddScoped<GetAllProductsQuery>();
builder.Services.AddScoped<GetDeletedProductsQuery>();
builder.Services.AddScoped<GetProductByIdQuery>();
builder.Services.AddScoped<CreateProductCommand>();
builder.Services.AddScoped<UpdateProductCommand>();
builder.Services.AddScoped<DeleteProductCommand>();
builder.Services.AddScoped<RestoreProductCommand>();

// Inventory commands
builder.Services.AddScoped<CreateReceiptCommand>();
builder.Services.AddScoped<CreateIssueCommand>();
builder.Services.AddScoped<CreateAdjustmentCommand>();
builder.Services.AddScoped<GetProductStockCardQuery>();
builder.Services.AddScoped<GetReorderRecommendationsQuery>();

// Purchasing draft commands/queries
builder.Services.AddScoped<CreatePurchaseRequestDraftCommand>();
builder.Services.AddScoped<UpdatePurchaseRequestDraftLineCommand>();
builder.Services.AddScoped<RemovePurchaseRequestDraftLineCommand>();
builder.Services.AddScoped<PreparePurchaseRequestDraftCommand>();
builder.Services.AddScoped<GetAllPurchaseRequestDraftsQuery>();
builder.Services.AddScoped<GetPurchaseRequestDraftByIdQuery>();

// Audit queries
builder.Services.AddScoped<GetAuditLogsQuery>();
builder.Services.AddScoped<GetAuditLogByIdQuery>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await ApplyDatabaseMigrationsAsync(app.Services, app.Environment);
await SeedAuthIfEnabledAsync(app.Services, app.Environment);

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services, IWebHostEnvironment environment)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (environment.IsDevelopment())
    {
        await db.Database.MigrateAsync();
        return;
    }

    if (environment.IsEnvironment("Testing"))
    {
        await db.Database.EnsureCreatedAsync();
    }
}

static bool IsApiRequest(PathString path)
    => path.StartsWithSegments("/api");

static async Task SeedAuthIfEnabledAsync(IServiceProvider services, IWebHostEnvironment environment)
{
    if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
    {
        return;
    }

    var delays = new[]
    {
        TimeSpan.Zero,
        TimeSpan.FromSeconds(3),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(15)
    };

    Exception? lastException = null;

    foreach (var delay in delays)
    {
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay);
        }

        try
        {
            await AuthSeeder.SeedAsync(services);
            return;
        }
        catch (Exception ex) when (IsTransientStartupDatabaseException(ex))
        {
            lastException = ex;
        }
    }

    throw new InvalidOperationException(
        "Authentication seed failed after multiple retries. Verify SQL Server health and try again.",
        lastException);
}

static bool IsTransientStartupDatabaseException(Exception exception)
{
    if (exception is TimeoutException)
    {
        return true;
    }

    if (exception is SqlException sqlException)
    {
        return sqlException.Number == -2
               || sqlException.Errors.Cast<SqlError>().Any(error => error.Number == -2);
    }

    return exception.InnerException is not null && IsTransientStartupDatabaseException(exception.InnerException);
}

public partial class Program;
