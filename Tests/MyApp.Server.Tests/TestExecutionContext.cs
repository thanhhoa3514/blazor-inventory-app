using MyApp.Server.Application.Common;

namespace MyApp.Server.Tests;

internal sealed class StaticCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly CurrentUserInfo _user;

    public StaticCurrentUserAccessor(string userId = "test-user-id", string userName = "test.user")
    {
        _user = new CurrentUserInfo(userId, userName);
    }

    public CurrentUserInfo GetRequiredCurrentUser() => _user;
}

internal sealed class NoOpAuditLogWriter : IAuditLogWriter
{
    public Task WriteAsync(string entityType, string entityId, string action, string summary, CancellationToken ct = default)
        => Task.CompletedTask;
}
