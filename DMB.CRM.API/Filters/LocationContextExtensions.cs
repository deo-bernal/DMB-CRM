using Dmb.Crm.Data.Repository.Interface.Location;

namespace Dmb.Crm.Api.Filters;

public static class LocationContextExtensions
{
    public static Guid RequireLocationId(this HttpContext httpContext)
    {
        if (httpContext.Items[LocationContextFilter.ItemKey] is UserLocationContext membership)
        {
            return membership.LocationId;
        }

        throw new InvalidOperationException("Location context is not available.");
    }
}
