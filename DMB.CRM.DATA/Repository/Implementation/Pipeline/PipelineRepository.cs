using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Entities;
using Dmb.Crm.Data.Repository.Interface.Pipeline;
using Dmb.Crm.Model.Dtos.Pipeline;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Repository.Implementation.Pipeline;

public class PipelineRepository : IPipelineRepository
{
    private readonly CrmContext _db;
    private readonly IMapper _mapper;

    public PipelineRepository(CrmContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ReadPipelineDto>> ListAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _db.Pipelines
            .AsNoTracking()
            .Include(p => p.Stages)
            .Where(p => p.LocationId == locationId)
            .OrderBy(p => p.Name)
            .ProjectTo<ReadPipelineDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReadPipelineDto?> GetAsync(Guid locationId, Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Pipelines
            .AsNoTracking()
            .Include(p => p.Stages)
            .Where(p => p.LocationId == locationId && p.Id == id)
            .ProjectTo<ReadPipelineDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ReadPipelineDto> CreateAsync(Guid locationId, CreatePipelineDto dto, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var pipeline = new Entities.Pipeline
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            Name = dto.Name.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        var stages = dto.Stages.Count == 0
            ? new[] { "New", "Qualified", "Proposal", "Won" }
                .Select((name, i) => new CreatePipelineStageDto { Name = name, SortOrder = i })
                .ToList()
            : dto.Stages.ToList();

        foreach (var stage in stages)
        {
            pipeline.Stages.Add(new PipelineStage
            {
                Id = Guid.NewGuid(),
                LocationId = locationId,
                PipelineId = pipeline.Id,
                Name = stage.Name.Trim(),
                SortOrder = stage.SortOrder
            });
        }

        _db.Pipelines.Add(pipeline);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetAsync(locationId, pipeline.Id, cancellationToken))!;
    }
}
