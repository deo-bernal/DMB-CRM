using Dmb.Crm.Api.Filters;
using Dmb.Crm.Model.Dtos.Tag;
using Dmb.Crm.Service.Interface.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.TagControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(LocationContextFilter))]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        return Ok(await _tagService.ListAsync(HttpContext.RequireLocationId(), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _tagService.CreateAsync(HttpContext.RequireLocationId(), dto, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagDto dto, CancellationToken cancellationToken)
    {
        var row = await _tagService.UpdateAsync(HttpContext.RequireLocationId(), id, dto, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _tagService.DeleteAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
