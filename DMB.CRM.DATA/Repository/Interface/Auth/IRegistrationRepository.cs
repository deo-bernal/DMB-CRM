using Dmb.Crm.Model.Dtos.Auth;

namespace Dmb.Crm.Data.Repository.Interface.Auth;

public interface IRegistrationRepository
{
    Task<RegisterWithActivationOutcome> RegisterWithActivationAsync(RegisterDto request, CancellationToken cancellationToken = default);
    Task<ActivateAccountOutcome> CompleteAccountActivationAsync(string? token, CancellationToken cancellationToken = default);
}
