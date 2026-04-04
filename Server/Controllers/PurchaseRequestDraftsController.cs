using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Purchasing.Commands;
using MyApp.Server.Application.Purchasing.Queries;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/purchase-request-drafts")]
[Authorize(Policy = AppPolicies.AdminOnly)]
public class PurchaseRequestDraftsController : ControllerBase
{
    private readonly GetAllPurchaseRequestDraftsQuery _getAll;
    private readonly GetPurchaseRequestDraftByIdQuery _getById;
    private readonly CreatePurchaseRequestDraftCommand _create;
    private readonly UpdatePurchaseRequestDraftLineCommand _updateLine;
    private readonly RemovePurchaseRequestDraftLineCommand _removeLine;
    private readonly PreparePurchaseRequestDraftCommand _prepare;
    private readonly ReviewPurchaseRequestDraftCommand _review;

    public PurchaseRequestDraftsController(
        GetAllPurchaseRequestDraftsQuery getAll,
        GetPurchaseRequestDraftByIdQuery getById,
        CreatePurchaseRequestDraftCommand create,
        UpdatePurchaseRequestDraftLineCommand updateLine,
        RemovePurchaseRequestDraftLineCommand removeLine,
        PreparePurchaseRequestDraftCommand prepare,
        ReviewPurchaseRequestDraftCommand review)
    {
        _getAll = getAll;
        _getById = getById;
        _create = create;
        _updateLine = updateLine;
        _removeLine = removeLine;
        _prepare = prepare;
        _review = review;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseRequestDraftListDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseRequestDraftDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _getById.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<PurchaseRequestDraftDetailDto>.Ok ok => Ok(ok.Value),
            AppResult<PurchaseRequestDraftDetailDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseRequestDraftDetailDto>> Create(CreatePurchaseRequestDraftRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _create.ExecuteAsync(request, cancellationToken);
        return result switch
        {
            AppResult<PurchaseRequestDraftDetailDto>.Ok ok => CreatedAtAction(nameof(GetById), new { id = ok.Value.Id }, ok.Value),
            AppResult<PurchaseRequestDraftDetailDto>.ValidationError err => BadRequest(err.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPut("{id:int}/lines/{lineId:int}")]
    public async Task<ActionResult<PurchaseRequestDraftDetailDto>> UpdateLine(int id, int lineId, UpdatePurchaseRequestDraftLineRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _updateLine.ExecuteAsync(id, lineId, request, cancellationToken);
        return result switch
        {
            AppResult<PurchaseRequestDraftDetailDto>.Ok ok => Ok(ok.Value),
            AppResult<PurchaseRequestDraftDetailDto>.NotFound => NotFound(),
            AppResult<PurchaseRequestDraftDetailDto>.ValidationError err => BadRequest(err.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}/lines/{lineId:int}")]
    public async Task<IActionResult> RemoveLine(int id, int lineId, CancellationToken cancellationToken)
    {
        var result = await _removeLine.ExecuteAsync(id, lineId, cancellationToken);
        return result switch
        {
            AppResult<Unit>.Ok => NoContent(),
            AppResult<Unit>.NotFound => NotFound(),
            AppResult<Unit>.ValidationError err => BadRequest(err.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{id:int}/prepare")]
    public async Task<ActionResult<PurchaseRequestDraftDetailDto>> Prepare(int id, CancellationToken cancellationToken)
    {
        var result = await _prepare.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<PurchaseRequestDraftDetailDto>.Ok ok => Ok(ok.Value),
            AppResult<PurchaseRequestDraftDetailDto>.NotFound => NotFound(),
            AppResult<PurchaseRequestDraftDetailDto>.ValidationError err => BadRequest(err.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpPost("{id:int}/review")]
    public async Task<ActionResult<PurchaseRequestDraftDetailDto>> Review(int id, CancellationToken cancellationToken)
    {
        var result = await _review.ExecuteAsync(id, cancellationToken);
        return result switch
        {
            AppResult<PurchaseRequestDraftDetailDto>.Ok ok => Ok(ok.Value),
            AppResult<PurchaseRequestDraftDetailDto>.NotFound => NotFound(),
            AppResult<PurchaseRequestDraftDetailDto>.ValidationError err => BadRequest(err.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
