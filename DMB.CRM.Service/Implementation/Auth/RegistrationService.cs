using Dmb.Crm.Data.Repository.Interface.Auth;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Service.Interface.Auth;

namespace Dmb.Crm.Service.Implementation.Auth;

public class RegistrationService : IRegistrationService
{
    private readonly IRegistrationRepository _repository;

    public RegistrationService(IRegistrationRepository repository)
    {
        _repository = repository;
    }

    public Task<RegisterWithActivationOutcome> RegisterWithActivationAsync(RegisterDto request, CancellationToken cancellationToken = default)
        => _repository.RegisterWithActivationAsync(request, cancellationToken);

    public Task<ActivateAccountOutcome> ActivateAccountAsync(string? token, CancellationToken cancellationToken = default)
        => _repository.CompleteAccountActivationAsync(token, cancellationToken);
}
