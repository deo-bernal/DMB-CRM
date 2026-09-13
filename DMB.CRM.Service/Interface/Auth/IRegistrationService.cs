using Dmb.Crm.Model.Dtos.Auth;

namespace Dmb.Crm.Service.Interface.Auth;

public interface IRegistrationService
{
    Task<RegisterWithActivationOutcome> RegisterWithActivationAsync(RegisterDto request, CancellationToken cancellationToken = default);
    Task<ActivateAccountOutcome> ActivateAccountAsync(string? token, CancellationToken cancellationToken = default);
}
