namespace Dmb.Crm.Data.Entities;

public class UserLocation
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
    public string Role { get; set; } = "user";
    public DateTimeOffset CreatedAt { get; set; }

    public CrmUser User { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
