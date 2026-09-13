using System.Security.Claims;
using Dmb.Crm.Service.Interface.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dmb.Crm.Api.Filters;

public sealed class LocationContextFilter : IAsyncActionFilter
{
    public const string HeaderName = "X-Location-Id";
    public const string ItemKey = "CrmLocationContext";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userIdValue = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var header = context.HttpContext.Request.Headers[HeaderName].FirstOrDefault();
        if (!Guid.TryParse(header, out var locationId))
        {
            context.Result = new BadRequestObjectResult(new { message = "X-Location-Id header is required." });
            return;
        }

        var locationService = context.HttpContext.RequestServices.GetRequiredService<ILocationService>();
        var membership = await locationService.GetMembershipAsync(userId, locationId, context.HttpContext.RequestAborted);
        if (membership is null)
        {
            context.Result = new ForbidResult();
            return;
        }

        context.HttpContext.Items[ItemKey] = membership;
        await next();
    }
}
