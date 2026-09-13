using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Entities;
using Dmb.Crm.Data.Repository.Interface.Contact;
using Dmb.Crm.Model.Dtos.Contact;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Repository.Implementation.Contact;

public class ContactRepository : IContactRepository
{
    private readonly CrmContext _db;
    private readonly IMapper _mapper;

    public ContactRepository(CrmContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReadContactDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await Query(locationId)
            .OrderByDescending(c => c.UpdatedAt)
            .ProjectTo<ReadContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReadContactDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        return await Query(locationId)
            .Where(c => c.Id == id)
            .ProjectTo<ReadContactDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ReadContactDto> CreateAsync(Guid locationId, CreateContactDto dto, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var row = new Entities.Contact
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            CompanyId = dto.CompanyId,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLowerInvariant(),
            Phone = dto.Phone,
            Source = dto.Source,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Contacts.Add(row);
        ApplyTags(row.Id, locationId, dto.TagIds);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetAsync(locationId, row.Id, cancellationToken))!;
    }

    public async Task<ReadContactDto?> UpdateAsync(Guid locationId, Guid id, UpdateContactDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _db.Contacts
            .Include(c => c.ContactTags)
            .FirstOrDefaultAsync(c => c.LocationId == locationId && c.Id == id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        row.CompanyId = dto.CompanyId;
        row.FirstName = dto.FirstName.Trim();
        row.LastName = dto.LastName.Trim();
        row.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLowerInvariant();
        row.Phone = dto.Phone;
        row.Source = dto.Source;
        row.UpdatedAt = DateTimeOffset.UtcNow;
        _db.ContactTags.RemoveRange(row.ContactTags);
        ApplyTags(row.Id, locationId, dto.TagIds);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetAsync(locationId, id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Contacts.FirstOrDefaultAsync(c => c.LocationId == locationId && c.Id == id, cancellationToken);
        if (row is null)
        {
            return false;
        }

        _db.Contacts.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<Entities.Contact> Query(Guid locationId)
    {
        return _db.Contacts
            .AsNoTracking()
            .Include(c => c.Company)
            .Include(c => c.ContactTags)
            .ThenInclude(ct => ct.Tag)
            .Where(c => c.LocationId == locationId);
    }

    private void ApplyTags(Guid contactId, Guid locationId, IReadOnlyList<Guid> tagIds)
    {
        var valid = _db.Tags
            .Where(t => t.LocationId == locationId && tagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToList();

        foreach (var tagId in valid)
        {
            _db.ContactTags.Add(new ContactTag { ContactId = contactId, TagId = tagId });
        }
    }
}
