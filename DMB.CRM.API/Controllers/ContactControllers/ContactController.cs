using Dmb.Crm.Api.Filters;
using Dmb.Crm.Model.Dtos.Contact;
using Dmb.Crm.Service.Interface.Contact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.ContactControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(LocationContextFilter))]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        return Ok(await _contactService.ListAsync(HttpContext.RequireLocationId(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var row = await _contactService.GetAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactDto dto, CancellationToken cancellationToken)
    {
        var row = await _contactService.CreateAsync(HttpContext.RequireLocationId(), dto, cancellationToken);
        return Ok(row);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactDto dto, CancellationToken cancellationToken)
    {
        var row = await _contactService.UpdateAsync(HttpContext.RequireLocationId(), id, dto, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _contactService.DeleteAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
