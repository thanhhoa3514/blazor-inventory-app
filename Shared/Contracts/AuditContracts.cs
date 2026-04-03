namespace MyApp.Shared.Contracts;

public record AuditLogDto(
    int Id,
    string EntityType,
    string EntityId,
    string Action,
    string? ActorUserId,
    string ActorUserName,
    string Summary,
    string? BeforeJson,
    string? AfterJson,
    string? ChangedFieldsJson,
    DateTime OccurredAtUtc);
