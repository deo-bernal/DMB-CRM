using Dmb.Crm.Model.Dtos.Auth;

namespace Dmb.Crm.Service.Interface.Auth;

public interface IAuthService
{
    Task<AuthTokenLoginResult> LoginWithJwtAsync(LoginDto model, CancellationToken cancellationToken = default);
    Task<LogoutWorkflowResult> LogoutAsync(string? username, string? jti, string? expClaim, Guid? userId, CancellationToken cancellationToken = default);
    Task<ForgotPasswordRequestStatus> RequestPasswordResetAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default);
    Task<PasswordResetCompletionStatus> CompletePasswordResetAsync(ResetPasswordDto request, CancellationToken cancellationToken = default);
    Task<bool> IsJtiRevokedAsync(string jti, CancellationToken cancellationToken = default);
    Task<LoggedInUserDto?> GetLoggedInUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(LoggedInUserDto? User, string? Error)> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminUserDto>> ListAgencyUsersAsync(Guid agencyId, Guid locationId, CancellationToken cancellationToken = default);
    Task<AdminUserDto?> UpdateAgencyUserAsync(Guid agencyId, Guid locationId, Guid actorUserId, bool actorIsSuperAdmin, Guid userId, UpdateAdminUserDto dto, CancellationToken cancellationToken = default);
    Task<string?> DeleteAgencyUserAsync(Guid agencyId, Guid actorUserId, Guid userId, CancellationToken cancellationToken = default);
}
