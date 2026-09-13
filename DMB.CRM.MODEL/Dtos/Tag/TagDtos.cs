namespace Dmb.Crm.Model.Dtos.Tag;

public class CreateTagDto
{
    public required string Name { get; set; }
    public string? Color { get; set; }
}

public class UpdateTagDto : CreateTagDto
{
}

public class ReadTagDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
