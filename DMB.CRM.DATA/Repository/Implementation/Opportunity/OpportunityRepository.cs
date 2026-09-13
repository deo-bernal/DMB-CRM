using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Repository.Interface.Opportunity;
using Dmb.Crm.Model.Dtos.Opportunity;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Repository.Implementation.Opportunity;

public class OpportunityRepository : IOpportunityRepository
{
    private readonly CrmContext _db;
    private readonly IMapper _mapper;

    public OpportunityRepository(CrmContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReadOpportunityDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await Query(locationId)
            .OrderBy(o => o.Stage.SortOrder)
            .ThenByDescending(o => o.UpdatedAt)
            .ProjectTo<ReadOpportunityDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReadOpportunityDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        return await Query(locationId)
            .Where(o => o.Id == id)
            .ProjectTo<ReadOpportunityDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ReadOpportunityDto> CreateAsync(Guid locationId, CreateOpportunityDto dto, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var row = new Entities.Opportunity
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            PipelineId = dto.PipelineId,
            StageId = dto.StageId,
            ContactId = dto.ContactId,
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            Value = dto.Value,
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "open" : dto.Status,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Opportunities.Add(row);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetAsync(locationId, row.Id, cancellationToken))!;
    }

    public async Task<ReadOpportunityDto?> UpdateAsync(Guid locationId, Guid id, UpdateOpportunityDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _db.Opportunities.FirstOrDefaultAsync(o => o.LocationId == locationId && o.Id == id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        row.PipelineId = dto.PipelineId;
        row.StageId = dto.StageId;
        row.ContactId = dto.ContactId;
        row.CompanyId = dto.CompanyId;
        row.Name = dto.Name.Trim();
        row.Value = dto.Value;
        row.Status = string.IsNullOrWhiteSpace(dto.Status) ? row.Status : dto.Status;
        row.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetAsync(locationId, id, cancellationToken);
    }

    public async Task<ReadOpportunityDto?> MoveAsync(Guid locationId, Guid id, Guid stageId, CancellationToken cancellationToken = default)
    {
        var row = await _db.Opportunities.FirstOrDefaultAsync(o => o.LocationId == locationId && o.Id == id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        var stage = await _db.PipelineStages.FirstOrDefaultAsync(
            s => s.Id == stageId && s.LocationId == locationId && s.PipelineId == row.PipelineId,
            cancellationToken);
        if (stage is null)
        {
            return null;
        }

        row.StageId = stageId;
        row.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetAsync(locationId, id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Opportunities.FirstOrDefaultAsync(o => o.LocationId == locationId && o.Id == id, cancellationToken);
        if (row is null)
        {
            return false;
        }

        _db.Opportunities.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<Entities.Opportunity> Query(Guid locationId)
    {
        return _db.Opportunities
            .AsNoTracking()
            .Include(o => o.Pipeline)
            .Include(o => o.Stage)
            .Where(o => o.LocationId == locationId);
    }
}
