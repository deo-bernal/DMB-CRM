using Dmb.Crm.Data.Repository.Interface.Location;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Model.Dtos.Location;

namespace Dmb.Crm.Service.Interface.Location;

public interface ILocationService
{
    Task<IReadOnlyList<LocationMembershipDto>> ListForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserLocationContext?> GetMembershipAsync(Guid userId, Guid locationId, CancellationToken cancellationToken = default);
    Task<LocationStatsDto> GetStatsAsync(Guid locationId, CancellationToken cancellationToken = default);
}
