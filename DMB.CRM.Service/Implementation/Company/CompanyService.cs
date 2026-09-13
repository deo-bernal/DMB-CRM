using Dmb.Crm.Data.Repository.Interface.Company;
using Dmb.Crm.Model.Dtos.Company;
using Dmb.Crm.Service.Interface.Company;

namespace Dmb.Crm.Service.Implementation.Company;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;

    public CompanyService(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ReadCompanyDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
        => _repository.ListAsync(locationId, cancellationToken);

    public Task<ReadCompanyDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.GetAsync(locationId, id, cancellationToken);

    public Task<ReadCompanyDto> CreateAsync(Guid locationId, CreateCompanyDto dto, CancellationToken cancellationToken = default)
        => _repository.CreateAsync(locationId, dto, cancellationToken);

    public Task<ReadCompanyDto?> UpdateAsync(Guid locationId, Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(locationId, id, dto, cancellationToken);

    public Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(locationId, id, cancellationToken);
}
