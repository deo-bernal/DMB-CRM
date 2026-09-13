namespace Dmb.Crm.Data.Entities;

public class ExternalLogin
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public CrmUser User { get; set; } = null!;
}
