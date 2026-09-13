namespace Dmb.Crm.Model.Dtos.Auth;

public class ExternalAuthCompleteRequest
{
    public string Ticket { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
}

public class ExternalAuthVerifyRequest
{
    public string Ticket { get; set; } = "";
    public string Code { get; set; } = "";
}

public class ExternalAuthStartResult
{
    public string? RedirectUrl { get; init; }
    public string? ErrorMessage { get; init; }
}

public class ExternalAuthVerifyResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int StatusCode { get; init; } = 400;
    public string? AccessToken { get; init; }
    public IReadOnlyList<LocationMembershipDto> Locations { get; init; } = [];
    public Guid? CurrentLocationId { get; init; }
}
