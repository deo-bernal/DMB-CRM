namespace Dmb.Crm.Data.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public Guid? CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Company? Company { get; set; }
    public ICollection<ContactTag> ContactTags { get; set; } = [];
}
