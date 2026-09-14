namespace Dmb.Crm.Model.Dtos.Auth;

public enum AuthTokenLoginStatus
{
    InvalidCredentials,
    AccountBlocked,
    Success
}

public enum ForgotPasswordRequestStatus
{
    Ok,
    EmailServiceUnavailable
}

public enum PasswordResetCompletionStatus
{
    Success,
    PasswordMismatch,
    InvalidOrExpiredToken
}

public enum RegisterWithActivationOutcome
{
    Success,
    DuplicateEmail,
    AgencyNotFound,
    ActivationEmailSendFailed
}

public enum ActivateAccountOutcome
{
    Success,
    InvalidOrExpiredToken
}

public class AuthTokenLoginResult
{
    public AuthTokenLoginStatus Status { get; set; }
    public string? AccessToken { get; set; }
    public string? BlockReason { get; set; }
    public IReadOnlyList<LocationMembershipDto> Locations { get; set; } = [];
    public Guid? CurrentLocationId { get; set; }
    public string? FirstName { get; set; }
    public bool IsSuperAdmin { get; set; }
}

public class LogoutWorkflowResult
{
    public string? Username { get; set; }
    public string Message { get; set; } = "Signed out.";
}

public class LocationMembershipDto
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = Dmb.Crm.Model.Roles.User;
}

public class LoggedInUserDto
{
    public Guid UserId { get; set; }
    public Guid AgencyId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ContactNo { get; set; }
    public bool Activated { get; set; }
    public bool IsSuperAdmin { get; set; }
}

public class UpdateOwnProfileDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? ContactNo { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}

public class AdminUserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ContactNo { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool Activated { get; set; }
    public bool IsSuperAdmin { get; set; }
}

public class UpdateAdminUserDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? ContactNo { get; set; }
    public required string Role { get; set; }
    public bool Activated { get; set; }
}
