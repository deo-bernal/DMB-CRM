using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Entities;
using Dmb.Crm.Data.Repository.Interface.Company;
using Dmb.Crm.Model.Dtos.Company;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Repository.Implementation.Company;

public class CompanyRepository : ICompanyRepository
{
    private readonly CrmContext _db;
    private readonly IMapper _mapper;

    public CompanyRepository(CrmContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReadCompanyDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _db.Companies
            .AsNoTracking()
            .Where(c => c.LocationId == locationId)
            .OrderByDescending(c => c.UpdatedAt)
            .ProjectTo<ReadCompanyDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReadCompanyDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.LocationId == locationId && c.Id == id, cancellationToken);
        return row is null ? null : _mapper.Map<ReadCompanyDto>(row);
    }

    public async Task<ReadCompanyDto> CreateAsync(Guid locationId, CreateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var row = new Entities.Company
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            Name = dto.Name.Trim(),
            Website = dto.Website,
            Phone = dto.Phone,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Companies.Add(row);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReadCompanyDto>(row);
    }

    public async Task<ReadCompanyDto?> UpdateAsync(Guid locationId, Guid id, UpdateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _db.Companies.FirstOrDefaultAsync(c => c.LocationId == locationId && c.Id == id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        row.Name = dto.Name.Trim();
        row.Website = dto.Website;
        row.Phone = dto.Phone;
        row.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReadCompanyDto>(row);
    }

    public async Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Companies.FirstOrDefaultAsync(c => c.LocationId == locationId && c.Id == id, cancellationToken);
        if (row is null)
        {
            return false;
        }

        _db.Companies.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
