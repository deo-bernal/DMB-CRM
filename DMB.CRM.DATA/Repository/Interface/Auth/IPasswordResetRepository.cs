using Dmb.Crm.Model.Dtos.Auth;

namespace Dmb.Crm.Data.Repository.Interface.Auth;

public interface IPasswordResetRepository
{
    Task<ForgotPasswordRequestStatus> RequestPasswordResetAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default);
    Task<PasswordResetCompletionStatus> CompletePasswordResetAsync(ResetPasswordDto request, CancellationToken cancellationToken = default);
}
