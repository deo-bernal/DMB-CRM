using Dmb.Crm.Api.Filters;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Service.Interface.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(LocationContextFilter))]
public class AdminUsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public AdminUsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("api/admin/users")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var membership = HttpContext.RequireMembership();
        if (membership.Role is not ("owner" or "admin"))
        {
            return Forbid();
        }

        return Ok(await _authService.ListAgencyUsersAsync(membership.AgencyId, membership.LocationId, cancellationToken));
    }

    [HttpPut("api/admin/users/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdminUserDto dto, CancellationToken cancellationToken)
    {
        var membership = HttpContext.RequireMembership();
        if (membership.Role is not ("owner" or "admin"))
        {
            return Forbid();
        }

        try
        {
            var updated = await _authService.UpdateAgencyUserAsync(
                membership.AgencyId,
                membership.LocationId,
                membership.UserId,
                membership.IsSuperAdmin,
                id,
                dto,
                cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("api/admin/users/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var membership = HttpContext.RequireMembership();
        if (membership.Role is not ("owner" or "admin"))
        {
            return Forbid();
        }

        var error = await _authService.DeleteAgencyUserAsync(membership.AgencyId, membership.UserId, id, cancellationToken);
        if (error is null)
        {
            return Ok(new { message = "User deleted.", deleted = true });
        }

        if (error.Contains("deactivated", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new { message = error, deactivated = true });
        }

        return BadRequest(new { message = error });
    }
}
