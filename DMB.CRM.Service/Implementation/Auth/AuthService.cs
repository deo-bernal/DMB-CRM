using Dmb.Crm.Data.Repository.Interface.Auth;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Service.Interface.Auth;

namespace Dmb.Crm.Service.Implementation.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordResetRepository _passwordResetRepository;

    public AuthService(IAuthRepository authRepository, IPasswordResetRepository passwordResetRepository)
    {
        _authRepository = authRepository;
        _passwordResetRepository = passwordResetRepository;
    }

    public Task<AuthTokenLoginResult> LoginWithJwtAsync(LoginDto model, CancellationToken cancellationToken = default)
        => _authRepository.LoginAndIssueJwtAsync(model, cancellationToken);

    public Task<LogoutWorkflowResult> LogoutAsync(string? username, string? jti, string? expClaim, Guid? userId, CancellationToken cancellationToken = default)
        => _authRepository.LogoutAsync(username, jti, expClaim, userId, cancellationToken);

    public Task<ForgotPasswordRequestStatus> RequestPasswordResetAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default)
        => _passwordResetRepository.RequestPasswordResetAsync(request, cancellationToken);

    public Task<PasswordResetCompletionStatus> CompletePasswordResetAsync(ResetPasswordDto request, CancellationToken cancellationToken = default)
        => _passwordResetRepository.CompletePasswordResetAsync(request, cancellationToken);

    public Task<bool> IsJtiRevokedAsync(string jti, CancellationToken cancellationToken = default)
        => _authRepository.IsJtiRevokedAsync(jti, cancellationToken);

    public Task<LoggedInUserDto?> GetLoggedInUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => _authRepository.GetLoggedInUserAsync(userId, cancellationToken);

    public Task<(LoggedInUserDto? User, string? Error)> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileDto dto, CancellationToken cancellationToken = default)
        => _authRepository.UpdateOwnProfileAsync(userId, dto, cancellationToken);

    public Task<IReadOnlyList<AdminUserDto>> ListAgencyUsersAsync(Guid agencyId, Guid locationId, CancellationToken cancellationToken = default)
        => _authRepository.ListAgencyUsersAsync(agencyId, locationId, cancellationToken);

    public Task<AdminUserDto?> UpdateAgencyUserAsync(Guid agencyId, Guid locationId, Guid actorUserId, bool actorIsSuperAdmin, Guid userId, UpdateAdminUserDto dto, CancellationToken cancellationToken = default)
        => _authRepository.UpdateAgencyUserAsync(agencyId, locationId, actorUserId, actorIsSuperAdmin, userId, dto, cancellationToken);

    public Task<string?> DeleteAgencyUserAsync(Guid agencyId, Guid actorUserId, Guid userId, CancellationToken cancellationToken = default)
        => _authRepository.DeleteAgencyUserAsync(agencyId, actorUserId, userId, cancellationToken);
}
