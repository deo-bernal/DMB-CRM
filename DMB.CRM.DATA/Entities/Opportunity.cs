namespace Dmb.Crm.Data.Entities;

public class Opportunity
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public Guid PipelineId { get; set; }
    public Guid StageId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Status { get; set; } = "open";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Pipeline Pipeline { get; set; } = null!;
    public PipelineStage Stage { get; set; } = null!;
    public Contact? Contact { get; set; }
    public Company? Company { get; set; }
}
