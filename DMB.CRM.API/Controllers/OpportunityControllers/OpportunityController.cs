using Dmb.Crm.Api.Filters;
using Dmb.Crm.Model.Dtos.Opportunity;
using Dmb.Crm.Service.Interface.Opportunity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.OpportunityControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(LocationContextFilter))]
public class OpportunityController : ControllerBase
{
    private readonly IOpportunityService _opportunityService;

    public OpportunityController(IOpportunityService opportunityService)
    {
        _opportunityService = opportunityService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        return Ok(await _opportunityService.ListAsync(HttpContext.RequireLocationId(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var row = await _opportunityService.GetAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOpportunityDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _opportunityService.CreateAsync(HttpContext.RequireLocationId(), dto, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOpportunityDto dto, CancellationToken cancellationToken)
    {
        var row = await _opportunityService.UpdateAsync(HttpContext.RequireLocationId(), id, dto, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveOpportunityDto dto, CancellationToken cancellationToken)
    {
        var row = await _opportunityService.MoveAsync(HttpContext.RequireLocationId(), id, dto.StageId, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _opportunityService.DeleteAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
