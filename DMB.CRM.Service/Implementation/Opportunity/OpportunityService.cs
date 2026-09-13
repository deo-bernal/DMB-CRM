using Dmb.Crm.Data.Repository.Interface.Opportunity;
using Dmb.Crm.Model.Dtos.Opportunity;
using Dmb.Crm.Service.Interface.Opportunity;

namespace Dmb.Crm.Service.Implementation.Opportunity;

public class OpportunityService : IOpportunityService
{
    private readonly IOpportunityRepository _repository;

    public OpportunityService(IOpportunityRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ReadOpportunityDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
        => _repository.ListAsync(locationId, cancellationToken);

    public Task<ReadOpportunityDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(locationId, id, cancellationToken);

    public Task<ReadOpportunityDto> CreateAsync(Guid locationId, CreateOpportunityDto dto, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(locationId, dto, cancellationToken);

    public Task<ReadOpportunityDto?> UpdateAsync(Guid locationId, Guid id, UpdateOpportunityDto dto, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(locationId, id, dto, cancellationToken);

    public Task<ReadOpportunityDto?> MoveAsync(Guid locationId, Guid id, Guid stageId, CancellationToken cancellationToken = default)
        => _repository.MoveAsync(locationId, id, stageId, cancellationToken);

    public Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(locationId, id, cancellationToken);
}
