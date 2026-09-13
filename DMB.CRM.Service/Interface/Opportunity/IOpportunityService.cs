using Dmb.Crm.Model.Dtos.Opportunity;

namespace Dmb.Crm.Service.Interface.Opportunity;

public interface IOpportunityService
{
    Task<IReadOnlyList<ReadOpportunityDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<ReadOpportunityDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
    Task<ReadOpportunityDto> CreateAsync(Guid locationId, CreateOpportunityDto dto, CancellationToken cancellationToken = default);
    Task<ReadOpportunityDto?> UpdateAsync(Guid locationId, Guid id, UpdateOpportunityDto dto, CancellationToken cancellationToken = default);
    Task<ReadOpportunityDto?> MoveAsync(Guid locationId, Guid id, Guid stageId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
}
