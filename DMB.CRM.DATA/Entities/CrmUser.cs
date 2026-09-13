namespace Dmb.Crm.Data.Entities;

public class CrmUser
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string? ContactNo { get; set; }
    public bool Activated { get; set; }
    public bool IsSuperAdmin { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Agency Agency { get; set; } = null!;
    public ICollection<UserLocation> UserLocations { get; set; } = [];
}
