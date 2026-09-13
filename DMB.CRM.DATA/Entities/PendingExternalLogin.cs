namespace Dmb.Crm.Data.Entities;

public class PendingExternalLogin
{
    public Guid Id { get; set; }
    public string Ticket { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Client { get; set; } = "web";
    public string? ReturnPath { get; set; }
    public string? CodeHash { get; set; }
    public DateTimeOffset? CodeExpiresAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
