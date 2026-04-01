using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Auth;
using MyApp.Server.Data;
using MyApp.Server.Services;
using MyApp.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

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
builder.Services.AddScoped<IInventoryService, InventoryService>();

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
await AuthSeeder.SeedAsync(app.Services);

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

public partial class Program;
