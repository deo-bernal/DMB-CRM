namespace Dmb.Crm.Data.Entities;

public class Location
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = "Asia/Manila";
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Agency Agency { get; set; } = null!;
    public ICollection<UserLocation> UserLocations { get; set; } = [];
}
