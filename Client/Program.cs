using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MyApp.Client;
using MyApp.Client.Features.Adjustments.Services;
using MyApp.Client.Features.Auth.Services;
using MyApp.Client.Features.Auth.State;
using MyApp.Client.Features.Categories.Services;
using MyApp.Client.Features.Customers.Services;
using MyApp.Client.Features.Inventory.Services;
using MyApp.Client.Features.Issues.Services;
using MyApp.Client.Features.Products.Services;
using MyApp.Client.Features.Receipts.Services;
using MyApp.Client.Features.Suppliers.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<CategoryApiClient>();
builder.Services.AddScoped<SupplierApiClient>();
builder.Services.AddScoped<CustomerApiClient>();
builder.Services.AddScoped<ProductApiClient>();
builder.Services.AddScoped<InventoryApiClient>();
builder.Services.AddScoped<ReceiptApiClient>();
builder.Services.AddScoped<IssueApiClient>();
builder.Services.AddScoped<AdjustmentApiClient>();
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
builder.Services.AddMudServices(configuration =>
{
    configuration.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
    configuration.SnackbarConfiguration.VisibleStateDuration = 4000;
    configuration.SnackbarConfiguration.ShowCloseIcon = true;
});

await builder.Build().RunAsync();
