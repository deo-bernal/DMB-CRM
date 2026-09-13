using Dmb.Crm.Data.Repository.Interface.Contact;
using Dmb.Crm.Model.Dtos.Contact;
using Dmb.Crm.Service.Interface.Contact;

namespace Dmb.Crm.Service.Implementation.Contact;

public class ContactService : IContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ReadContactDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
        => _repository.ListAsync(locationId, cancellationToken);

    public Task<ReadContactDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(locationId, id, cancellationToken);

    public Task<ReadContactDto> CreateAsync(Guid locationId, CreateContactDto dto, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(locationId, dto, cancellationToken);

    public Task<ReadContactDto?> UpdateAsync(Guid locationId, Guid id, UpdateContactDto dto, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(locationId, id, dto, cancellationToken);

    public Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(locationId, id, cancellationToken);
}
