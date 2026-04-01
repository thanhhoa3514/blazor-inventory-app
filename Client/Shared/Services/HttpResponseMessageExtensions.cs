namespace MyApp.Client.Shared.Services;

public static class HttpResponseMessageExtensions
{
    public static async Task<string> ReadErrorMessageAsync(this HttpResponseMessage response, string fallback = "Request failed.")
    {
        var raw = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(raw) ? fallback : raw.Trim().Trim('"');
    }
}
