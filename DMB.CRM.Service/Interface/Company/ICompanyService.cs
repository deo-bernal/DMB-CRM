using Dmb.Crm.Model.Dtos.Company;

namespace Dmb.Crm.Service.Interface.Company;

public interface ICompanyService
{
    Task<IReadOnlyList<ReadCompanyDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default);
    Task<ReadCompanyDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
    Task<ReadCompanyDto> CreateAsync(Guid locationId, CreateCompanyDto dto, CancellationToken cancellationToken = default);
    Task<ReadCompanyDto?> UpdateAsync(Guid locationId, Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default);
}
