using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Products.Commands;
using MyApp.Server.Application.Products.Queries;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/products")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class ProductsController : ControllerBase
{
    private readonly GetAllProductsQuery _getAll;
    private readonly GetProductByIdQuery _getById;
    private readonly CreateProductCommand _create;
    private readonly UpdateProductCommand _update;
    private readonly DeleteProductCommand _delete;

    public ProductsController(
        GetAllProductsQuery getAll,
        GetProductByIdQuery getById,
        CreateProductCommand create,
        UpdateProductCommand update,
        DeleteProductCommand delete)
    {
        _getAll = getAll;
        _getById = getById;
        _create = create;
        _update = update;
        _delete = delete;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<ProductDto>.Ok ok => Ok(ok.Value),
            AppResult<ProductDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _create.ExecuteAsync(request, cancellationToken);
        return result switch
        {
            AppResult<ProductDto>.Ok ok => CreatedAtAction(nameof(GetById), new { id = ok.Value.Id }, ok.Value),
            AppResult<ProductDto>.Conflict conflict => BadRequest(conflict.Message),
            AppResult<ProductDto>.ValidationError validationError => BadRequest(validationError.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _update.ExecuteAsync(id, request, cancellationToken);
        return result switch
        {
            AppResult<ProductDto>.Ok ok => Ok(ok.Value),
            AppResult<ProductDto>.NotFound => NotFound(),
            AppResult<ProductDto>.Conflict conflict => BadRequest(conflict.Message),
            AppResult<ProductDto>.ValidationError validationError => BadRequest(validationError.Message),
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
}
