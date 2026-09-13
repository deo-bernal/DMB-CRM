using Dmb.Crm.Api.Filters;
using Dmb.Crm.Model.Dtos.Pipeline;
using Dmb.Crm.Service.Interface.Pipeline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.PipelineControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(LocationContextFilter))]
public class PipelineController : ControllerBase
{
    private readonly IPipelineService _pipelineService;

    public PipelineController(IPipelineService pipelineService)
    {
        _pipelineService = pipelineService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        return Ok(await _pipelineService.ListAsync(HttpContext.RequireLocationId(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var row = await _pipelineService.GetAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePipelineDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _pipelineService.CreateAsync(HttpContext.RequireLocationId(), dto, cancellationToken));
    }
}
