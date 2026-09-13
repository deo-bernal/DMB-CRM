namespace Dmb.Crm.Model.Dtos.Company;

public class CreateCompanyDto
{
    public required string Name { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
}

public class UpdateCompanyDto : CreateCompanyDto
{
}

public class ReadCompanyDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
