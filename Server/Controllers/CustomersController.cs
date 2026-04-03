using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Customers.Commands;
using MyApp.Server.Application.Customers.Queries;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class CustomersController : ControllerBase
{
    private readonly GetAllCustomersQuery _getAll;
    private readonly GetDeletedCustomersQuery _getDeleted;
    private readonly GetCustomerByIdQuery _getById;
    private readonly CreateCustomerCommand _create;
    private readonly UpdateCustomerCommand _update;
    private readonly DeactivateCustomerCommand _deactivate;
    private readonly SoftDeleteCustomerCommand _softDelete;
    private readonly RestoreCustomerCommand _restore;

    public CustomersController(
        GetAllCustomersQuery getAll,
        GetDeletedCustomersQuery getDeleted,
        GetCustomerByIdQuery getById,
        CreateCustomerCommand create,
        UpdateCustomerCommand update,
        DeactivateCustomerCommand deactivate,
        SoftDeleteCustomerCommand softDelete,
        RestoreCustomerCommand restore)
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
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("deleted")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetDeleted(CancellationToken cancellationToken)
        => Ok(await _getDeleted.ExecuteAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<CustomerDto>.Ok ok => Ok(ok.Value),
            AppResult<CustomerDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _create.ExecuteAsync(request, cancellationToken);
        return result switch
        {
            AppResult<CustomerDto>.Ok ok => CreatedAtAction(nameof(GetById), new { id = ok.Value.Id }, ok.Value),
            AppResult<CustomerDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<CustomerDto>> Update(int id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _update.ExecuteAsync(id, request, cancellationToken);
        return result switch
        {
            AppResult<CustomerDto>.Ok ok => Ok(ok.Value),
            AppResult<CustomerDto>.NotFound => NotFound(),
            AppResult<CustomerDto>.Conflict conflict => BadRequest(conflict.Message),
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
    public async Task<ActionResult<CustomerDto>> Restore(int id, CancellationToken cancellationToken)
    {
        var result = await _restore.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<CustomerDto>.Ok ok => Ok(ok.Value),
            AppResult<CustomerDto>.NotFound => NotFound(),
            AppResult<CustomerDto>.ValidationError validationError => BadRequest(validationError.Message),
            AppResult<CustomerDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
