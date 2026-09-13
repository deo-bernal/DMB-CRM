using Dmb.Crm.Model.Dtos.Pipeline;

namespace Dmb.Crm.Data.Repository.Interface.Pipeline;

public interface IPipelineRepository
{
    Task<IReadOnlyList<ReadPipelineDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<ReadPipelineDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
    Task<ReadPipelineDto> CreateAsync(Guid locationId, CreatePipelineDto dto, CancellationToken cancellationToken = default);
}
