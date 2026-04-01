using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Inventory.Commands;
using MyApp.Server.Auth;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/adjustments")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class AdjustmentsController : ControllerBase
{
    private readonly IStockAdjustmentRepository _repo;
    private readonly CreateAdjustmentCommand _create;

    public AdjustmentsController(IStockAdjustmentRepository repo, CreateAdjustmentCommand create)
    {
        _repo = repo;
        _create = create;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockAdjustmentListDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _repo.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockAdjustmentDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _repo.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.WarehouseOperations)]
    public async Task<ActionResult<StockAdjustmentDetailDto>> Create(CreateStockAdjustmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _create.ExecuteAsync(request, cancellationToken);
        return result switch
        {
            AppResult<StockAdjustmentDetailDto>.Ok ok => CreatedAtAction(nameof(GetById), new { id = ok.Value.Id }, ok.Value),
            AppResult<StockAdjustmentDetailDto>.ValidationError err => BadRequest(err.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
