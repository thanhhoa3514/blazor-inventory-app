using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Audit.Queries;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Policy = AppPolicies.AdminOnly)]
public class AuditLogsController : ControllerBase
{
    private readonly GetAuditLogsQuery _getAll;

    public AuditLogsController(GetAuditLogsQuery getAll)
    {
        _getAll = getAll;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll(
        [FromQuery] string? entityType,
        [FromQuery] string? entityId,
        [FromQuery] string? actorUserName,
        [FromQuery] string? action,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(entityType, entityId, actorUserName, action, fromUtc, toUtc, cancellationToken));
}
