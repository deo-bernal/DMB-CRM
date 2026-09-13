using Dmb.Crm.Data.Repository.Interface.Tag;
using Dmb.Crm.Model.Dtos.Tag;
using Dmb.Crm.Service.Interface.Tag;

namespace Dmb.Crm.Service.Implementation.Tag;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ReadTagDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
        => _repository.ListAsync(locationId, cancellationToken);

    public Task<ReadTagDto> CreateAsync(Guid locationId, CreateTagDto dto, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(locationId, dto, cancellationToken);

    public Task<ReadTagDto?> UpdateAsync(Guid locationId, Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(locationId, id, dto, cancellationToken);

    public Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(locationId, id, cancellationToken);
}
