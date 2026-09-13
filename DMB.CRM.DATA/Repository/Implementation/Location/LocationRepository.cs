using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Repository.Interface.Location;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Model.Dtos.Location;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Repository.Implementation.Location;

public class LocationRepository : ILocationRepository
{
    private readonly CrmContext _db;

    public LocationRepository(CrmContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<LocationMembershipDto>> ListForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.UserLocations
            .AsNoTracking()
            .Where(ul => ul.UserId == userId && ul.Location.IsActive)
            .Select(ul => new LocationMembershipDto
            {
                LocationId = ul.LocationId,
                Name = ul.Location.Name,
                Role = ul.Role
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserLocationContext?> GetMembershipAsync(Guid userId, Guid locationId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (user.IsSuperAdmin)
        {
            var location = await _db.Locations.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == locationId && l.AgencyId == user.AgencyId, cancellationToken);
            if (location is null)
            {
                return null;
            }

            return new UserLocationContext
            {
                UserId = user.Id,
                AgencyId = user.AgencyId,
                LocationId = location.Id,
                Role = Model.Roles.Owner,
                IsSuperAdmin = true
            };
        }

        var membership = await _db.UserLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(ul => ul.UserId == userId && ul.LocationId == locationId, cancellationToken);

        if (membership is null)
        {
            return null;
        }

        return new UserLocationContext
        {
            UserId = user.Id,
            AgencyId = user.AgencyId,
            LocationId = membership.LocationId,
            Role = membership.Role,
            IsSuperAdmin = false
        };
    }

    public async Task<LocationStatsDto> GetStatsAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return new LocationStatsDto
        {
            ContactCount = await _db.Contacts.CountAsync(c => c.LocationId == locationId, cancellationToken),
            CompanyCount = await _db.Companies.CountAsync(c => c.LocationId == locationId, cancellationToken),
            OpenOpportunityCount = await _db.Opportunities.CountAsync(
                o => o.LocationId == locationId && o.Status == "open", cancellationToken),
            OpenPipelineValue = await _db.Opportunities
                .Where(o => o.LocationId == locationId && o.Status == "open")
                .SumAsync(o => (decimal?)o.Value, cancellationToken) ?? 0
        };
    }
}
