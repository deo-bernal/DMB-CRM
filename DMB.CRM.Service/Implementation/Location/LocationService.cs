using Dmb.Crm.Data.Repository.Interface.Location;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Model.Dtos.Location;
using Dmb.Crm.Service.Interface.Location;

namespace Dmb.Crm.Service.Implementation.Location;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _repository;

    public LocationService(ILocationRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<LocationMembershipDto>> ListForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => _repository.ListForUserAsync(userId, cancellationToken);

    public Task<UserLocationContext?> GetMembershipAsync(Guid userId, Guid locationId, CancellationToken cancellationToken = default)
        => _repository.GetMembershipAsync(userId, locationId, cancellationToken);

    public Task<LocationStatsDto> GetStatsAsync(Guid locationId, CancellationToken cancellationToken = default)
        => _repository.GetStatsAsync(locationId, cancellationToken);
}
