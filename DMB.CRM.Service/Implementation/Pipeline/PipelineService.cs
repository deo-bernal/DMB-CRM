using Dmb.Crm.Data.Repository.Interface.Pipeline;
using Dmb.Crm.Model.Dtos.Pipeline;
using Dmb.Crm.Service.Interface.Pipeline;

namespace Dmb.Crm.Service.Implementation.Pipeline;

public class PipelineService : IPipelineService
{
    private readonly IPipelineRepository _repository;

    public PipelineService(IPipelineRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ReadPipelineDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
        => _repository.ListAsync(locationId, cancellationToken);

    public Task<ReadPipelineDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(locationId, id, cancellationToken);

    public Task<ReadPipelineDto> CreateAsync(Guid locationId, CreatePipelineDto dto, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(locationId, dto, cancellationToken);
}
