namespace Dmb.Crm.Data.Entities;

public class Pipeline
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<PipelineStage> Stages { get; set; } = [];
}

public class PipelineStage
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public Guid PipelineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Pipeline Pipeline { get; set; } = null!;
}
