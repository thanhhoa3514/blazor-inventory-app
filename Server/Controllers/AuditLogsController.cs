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
    private readonly GetAuditLogByIdQuery _getById;

    public AuditLogsController(GetAuditLogsQuery getAll, GetAuditLogByIdQuery getById)
    {
        _getAll = getAll;
        _getById = getById;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuditLogDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            MyApp.Server.Application.Common.AppResult<AuditLogDto>.Ok ok => Ok(ok.Value),
            MyApp.Server.Application.Common.AppResult<AuditLogDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
