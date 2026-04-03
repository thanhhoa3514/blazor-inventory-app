using System.Net.Http.Json;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Audit.Services;

public sealed class AuditLogApiClient
{
    private readonly HttpClient _httpClient;

    public AuditLogApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AuditLogDto>> GetAllAsync(
        string? entityType = null,
        string? entityId = null,
        string? actorUserName = null,
        string? action = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(entityType)) query.Add($"entityType={Uri.EscapeDataString(entityType)}");
        if (!string.IsNullOrWhiteSpace(entityId)) query.Add($"entityId={Uri.EscapeDataString(entityId)}");
        if (!string.IsNullOrWhiteSpace(actorUserName)) query.Add($"actorUserName={Uri.EscapeDataString(actorUserName)}");
        if (!string.IsNullOrWhiteSpace(action)) query.Add($"action={Uri.EscapeDataString(action)}");
        if (fromUtc.HasValue) query.Add($"fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}");
        if (toUtc.HasValue) query.Add($"toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}");

        var path = "api/audit-logs";
        if (query.Count > 0) path += "?" + string.Join("&", query);
        return await _httpClient.GetFromJsonAsync<List<AuditLogDto>>(path, cancellationToken) ?? [];
    }

    public async Task<AuditLogDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/audit-logs/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuditLogDto>(cancellationToken);
    }
}
