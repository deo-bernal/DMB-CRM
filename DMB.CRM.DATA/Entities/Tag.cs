namespace Dmb.Crm.Data.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }

    public ICollection<ContactTag> ContactTags { get; set; } = [];
}

public class ContactTag
{
    public Guid ContactId { get; set; }
    public Guid TagId { get; set; }

    public Contact Contact { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
