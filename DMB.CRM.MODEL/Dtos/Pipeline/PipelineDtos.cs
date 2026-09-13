namespace Dmb.Crm.Model.Dtos.Pipeline;

public class CreatePipelineDto
{
    public required string Name { get; set; }
    public IReadOnlyList<CreatePipelineStageDto> Stages { get; set; } = [];
}

public class CreatePipelineStageDto
{
    public required string Name { get; set; }
    public int SortOrder { get; set; }
}

public class UpdatePipelineDto : CreatePipelineDto
{
}

public class ReadPipelineDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<ReadPipelineStageDto> Stages { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class ReadPipelineStageDto
{
    public Guid Id { get; set; }
    public Guid PipelineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
