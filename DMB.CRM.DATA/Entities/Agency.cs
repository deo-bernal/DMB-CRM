namespace Dmb.Crm.Data.Entities;

public class Agency
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<CrmUser> Users { get; set; } = [];
}
