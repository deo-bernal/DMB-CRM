namespace Dmb.Crm.Model.Dtos.Contact;

public class CreateContactDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public Guid? CompanyId { get; set; }
    public IReadOnlyList<Guid> TagIds { get; set; } = [];
}

public class UpdateContactDto : CreateContactDto
{
}

public class ReadContactDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public IReadOnlyList<ReadTagRefDto> Tags { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class ReadTagRefDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}
