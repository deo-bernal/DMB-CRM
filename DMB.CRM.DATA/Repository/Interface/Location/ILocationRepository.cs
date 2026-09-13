using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Model.Dtos.Location;

namespace Dmb.Crm.Data.Repository.Interface.Location;

public interface ILocationRepository
{
    Task<IReadOnlyList<LocationMembershipDto>> ListForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserLocationContext?> GetMembershipAsync(Guid userId, Guid locationId, CancellationToken cancellationToken = default);
    Task<LocationStatsDto> GetStatsAsync(Guid locationId, CancellationToken cancellationToken = default);
}

public class UserLocationContext
{
    public Guid UserId { get; set; }
    public Guid AgencyId { get; set; }
    public Guid LocationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsSuperAdmin { get; set; }
}
