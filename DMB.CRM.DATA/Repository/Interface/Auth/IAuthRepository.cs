using Dmb.Crm.Model.Dtos.Auth;

namespace Dmb.Crm.Data.Repository.Interface.Auth;

public interface IAuthRepository
{
    Task<AuthTokenLoginResult> LoginAndIssueJwtAsync(LoginDto model, CancellationToken cancellationToken = default);
    Task<LogoutWorkflowResult> LogoutAsync(string? username, string? jti, string? expClaim, Guid? userId, CancellationToken cancellationToken = default);
    Task<bool> IsJtiRevokedAsync(string jti, CancellationToken cancellationToken = default);
    Task RevokeJtiAsync(string jti, DateTimeOffset expiresAt, Guid? userId, CancellationToken cancellationToken = default);
    (string PasswordHash, string PasswordSalt) CreatePasswordHash(string password);
    bool VerifyPassword(string password, string passwordSalt, string passwordHash);
    Task<IReadOnlyList<LocationMembershipDto>> GetUserLocationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<AuthTokenLoginResult> IssueJwtForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<LoggedInUserDto?> GetLoggedInUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(LoggedInUserDto? User, string? Error)> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminUserDto>> ListAgencyUsersAsync(Guid agencyId, Guid locationId, CancellationToken cancellationToken = default);
    Task<AdminUserDto?> UpdateAgencyUserAsync(Guid agencyId, Guid locationId, Guid actorUserId, bool actorIsSuperAdmin, Guid userId, UpdateAdminUserDto dto, CancellationToken cancellationToken = default);
    Task<string?> DeleteAgencyUserAsync(Guid agencyId, Guid actorUserId, Guid userId, CancellationToken cancellationToken = default);
}
