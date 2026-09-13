using Dmb.Crm.Model.Dtos.Tag;

namespace Dmb.Crm.Data.Repository.Interface.Tag;

public interface ITagRepository
{
    Task<IReadOnlyList<ReadTagDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<ReadTagDto> CreateAsync(Guid locationId, CreateTagDto dto, CancellationToken cancellationToken = default);
    Task<ReadTagDto?> UpdateAsync(Guid locationId, Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
}
