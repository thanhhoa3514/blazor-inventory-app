using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Suppliers.Commands;
using MyApp.Server.Application.Suppliers.Queries;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class SuppliersController : ControllerBase
{
    private readonly GetAllSuppliersQuery _getAll;
    private readonly GetDeletedSuppliersQuery _getDeleted;
    private readonly GetSupplierByIdQuery _getById;
    private readonly CreateSupplierCommand _create;
    private readonly UpdateSupplierCommand _update;
    private readonly DeactivateSupplierCommand _deactivate;
    private readonly SoftDeleteSupplierCommand _softDelete;
    private readonly RestoreSupplierCommand _restore;

    public SuppliersController(
        GetAllSuppliersQuery getAll,
        GetDeletedSuppliersQuery getDeleted,
        GetSupplierByIdQuery getById,
        CreateSupplierCommand create,
        UpdateSupplierCommand update,
        DeactivateSupplierCommand deactivate,
        SoftDeleteSupplierCommand softDelete,
        RestoreSupplierCommand restore)
    {
        _getAll = getAll;
        _getDeleted = getDeleted;
        _getById = getById;
        _create = create;
        _update = update;
        _deactivate = deactivate;
        _softDelete = softDelete;
        _restore = restore;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("deleted")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetDeleted(CancellationToken cancellationToken)
        => Ok(await _getDeleted.ExecuteAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<SupplierDto>.Ok ok => Ok(ok.Value),
            AppResult<SupplierDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _create.ExecuteAsync(request, cancellationToken);
        return result switch
        {
            AppResult<SupplierDto>.Ok ok => CreatedAtAction(nameof(GetById), new { id = ok.Value.Id }, ok.Value),
            AppResult<SupplierDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<SupplierDto>> Update(int id, UpdateSupplierRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _update.ExecuteAsync(id, request, cancellationToken);
        return result switch
        {
            AppResult<SupplierDto>.Ok ok => Ok(ok.Value),
            AppResult<SupplierDto>.NotFound => NotFound(),
            AppResult<SupplierDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var result = await _deactivate.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<Unit>.Ok => NoContent(),
            AppResult<Unit>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{id:int}/soft-delete")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> SoftDelete(int id, CancellationToken cancellationToken)
    {
        var result = await _softDelete.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<Unit>.Ok => NoContent(),
            AppResult<Unit>.NotFound => NotFound(),
            AppResult<Unit>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{id:int}/restore")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<SupplierDto>> Restore(int id, CancellationToken cancellationToken)
    {
        var result = await _restore.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<SupplierDto>.Ok ok => Ok(ok.Value),
            AppResult<SupplierDto>.NotFound => NotFound(),
            AppResult<SupplierDto>.ValidationError validationError => BadRequest(validationError.Message),
            AppResult<SupplierDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
