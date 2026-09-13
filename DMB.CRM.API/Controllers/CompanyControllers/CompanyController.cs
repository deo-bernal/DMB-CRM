using Dmb.Crm.Api.Filters;
using Dmb.Crm.Model.Dtos.Company;
using Dmb.Crm.Service.Interface.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.CompanyControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(LocationContextFilter))]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        return Ok(await _companyService.ListAsync(HttpContext.RequireLocationId(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var row = await _companyService.GetAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto, CancellationToken cancellationToken)
    {
        return Ok(await _companyService.CreateAsync(HttpContext.RequireLocationId(), dto, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyDto dto, CancellationToken cancellationToken)
    {
        var row = await _companyService.UpdateAsync(HttpContext.RequireLocationId(), id, dto, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _companyService.DeleteAsync(HttpContext.RequireLocationId(), id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
