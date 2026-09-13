using Dmb.Crm.Model.Dtos.Contact;

namespace Dmb.Crm.Data.Repository.Interface.Contact;

public interface IContactRepository
{
    Task<IReadOnlyList<ReadContactDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<ReadContactDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
    Task<ReadContactDto> CreateAsync(Guid locationId, CreateContactDto dto, CancellationToken cancellationToken = default);
    Task<ReadContactDto?> UpdateAsync(Guid locationId, Guid id, UpdateContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
}
