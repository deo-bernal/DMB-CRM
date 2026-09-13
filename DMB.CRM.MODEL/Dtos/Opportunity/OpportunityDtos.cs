namespace Dmb.Crm.Model.Dtos.Opportunity;

public class CreateOpportunityDto
{
    public required string Name { get; set; }
    public Guid PipelineId { get; set; }
    public Guid StageId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? CompanyId { get; set; }
    public decimal Value { get; set; }
    public string Status { get; set; } = "open";
}

public class UpdateOpportunityDto : CreateOpportunityDto
{
}

public class MoveOpportunityDto
{
    public Guid StageId { get; set; }
}

public class ReadOpportunityDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public Guid PipelineId { get; set; }
    public string? PipelineName { get; set; }
    public Guid StageId { get; set; }
    public string? StageName { get; set; }
    public int SortOrder { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Status { get; set; } = "open";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
