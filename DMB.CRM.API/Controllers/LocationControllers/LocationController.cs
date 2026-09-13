using System.Security.Claims;
using Dmb.Crm.Api.Filters;
using Dmb.Crm.Service.Interface.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.LocationControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var result = await _locationService.ListForUserAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stats")]
    [ServiceFilter(typeof(LocationContextFilter))]
    public async Task<IActionResult> Stats(CancellationToken cancellationToken)
    {
        var result = await _locationService.GetStatsAsync(HttpContext.RequireLocationId(), cancellationToken);
        return Ok(result);
    }
}
