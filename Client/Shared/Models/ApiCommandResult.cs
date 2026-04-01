namespace MyApp.Client.Shared.Models;

public sealed record ApiCommandResult(bool Success, string? ErrorMessage = null)
{
    public static ApiCommandResult Ok() => new(true);

    public static ApiCommandResult Fail(string errorMessage) => new(false, errorMessage);
}
