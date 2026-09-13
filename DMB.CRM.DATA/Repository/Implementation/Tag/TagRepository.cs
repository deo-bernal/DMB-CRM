using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Repository.Interface.Tag;
using Dmb.Crm.Model.Dtos.Tag;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Repository.Implementation.Tag;

public class TagRepository : ITagRepository
{
    private readonly CrmContext _db;
    private readonly IMapper _mapper;

    public TagRepository(CrmContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReadTagDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _db.Tags
            .AsNoTracking()
            .Where(t => t.LocationId == locationId)
            .OrderBy(t => t.Name)
            .ProjectTo<ReadTagDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReadTagDto> CreateAsync(Guid locationId, CreateTagDto dto, CancellationToken cancellationToken = default)
    {
        var row = new Entities.Tag
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            Name = dto.Name.Trim(),
            Color = dto.Color
        };
        _db.Tags.Add(row);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReadTagDto>(row);
    }

    public async Task<ReadTagDto?> UpdateAsync(Guid locationId, Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _db.Tags.FirstOrDefaultAsync(t => t.LocationId == locationId && t.Id == id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        row.Name = dto.Name.Trim();
        row.Color = dto.Color;
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReadTagDto>(row);
    }

    public async Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Tags.FirstOrDefaultAsync(t => t.LocationId == locationId && t.Id == id, cancellationToken);
        if (row is null)
        {
            return false;
        }

        _db.Tags.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
