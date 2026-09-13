using Dmb.Crm.Model.Dtos.Pipeline;

namespace Dmb.Crm.Service.Interface.Pipeline;

public interface IPipelineService
{
    Task<IReadOnlyList<ReadPipelineDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<ReadPipelineDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
    Task<ReadPipelineDto> CreateAsync(Guid locationId, CreatePipelineDto dto, CancellationToken cancellationToken = default);
}
