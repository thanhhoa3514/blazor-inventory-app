using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Categories.Commands;
using MyApp.Server.Application.Categories.Queries;
using MyApp.Server.Application.Common;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class CategoriesController : ControllerBase
{
    private readonly GetAllCategoriesQuery _getAll;
    private readonly GetDeletedCategoriesQuery _getDeleted;
    private readonly GetCategoryByIdQuery _getById;
    private readonly CreateCategoryCommand _create;
    private readonly UpdateCategoryCommand _update;
    private readonly DeleteCategoryCommand _delete;
    private readonly RestoreCategoryCommand _restore;

    public CategoriesController(
        GetAllCategoriesQuery getAll,
        GetDeletedCategoriesQuery getDeleted,
        GetCategoryByIdQuery getById,
        CreateCategoryCommand create,
        UpdateCategoryCommand update,
        DeleteCategoryCommand delete,
        RestoreCategoryCommand restore)
    {
        _getAll = getAll;
        _getDeleted = getDeleted;
        _getById = getById;
        _create = create;
        _update = update;
        _delete = delete;
        _restore = restore;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("deleted")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetDeleted(CancellationToken cancellationToken)
        => Ok(await _getDeleted.ExecuteAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<CategoryDto>.Ok ok => Ok(ok.Value),
            AppResult<CategoryDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _create.ExecuteAsync(request, cancellationToken);
        return result switch
        {
            AppResult<CategoryDto>.Ok ok => CreatedAtAction(nameof(GetById), new { id = ok.Value.Id }, ok.Value),
            AppResult<CategoryDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _update.ExecuteAsync(id, request, cancellationToken);
        return result switch
        {
            AppResult<CategoryDto>.Ok ok => Ok(ok.Value),
            AppResult<CategoryDto>.NotFound => NotFound(),
            AppResult<CategoryDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _delete.ExecuteAsync(id, cancellationToken);
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
    public async Task<ActionResult<CategoryDto>> Restore(int id, CancellationToken cancellationToken)
    {
        var result = await _restore.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<CategoryDto>.Ok ok => Ok(ok.Value),
            AppResult<CategoryDto>.NotFound => NotFound(),
            AppResult<CategoryDto>.ValidationError validationError => BadRequest(validationError.Message),
            AppResult<CategoryDto>.Conflict conflict => BadRequest(conflict.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
