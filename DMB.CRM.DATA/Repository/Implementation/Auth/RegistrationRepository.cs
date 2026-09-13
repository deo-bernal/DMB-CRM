using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Entities;
using Dmb.Crm.Data.Repository.Interface.Auth;
using Dmb.Crm.Data.Security;
using Dmb.Crm.Model.Abstractions;
using Dmb.Crm.Model.Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dmb.Crm.Data.Repository.Implementation.Auth;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly CrmContext _db;
    private readonly IAuthRepository _authRepository;
    private readonly IActivationEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegistrationRepository> _logger;

    public RegistrationRepository(
        CrmContext db,
        IAuthRepository authRepository,
        IActivationEmailSender emailSender,
        IConfiguration configuration,
        ILogger<RegistrationRepository> logger)
    {
        _db = db;
        _authRepository = authRepository;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<RegisterWithActivationOutcome> RegisterWithActivationAsync(
        RegisterDto request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var slug = string.IsNullOrWhiteSpace(request.AgencySlug) ? "dmb" : request.AgencySlug.Trim().ToLowerInvariant();

        var agency = await _db.Agencies.FirstOrDefaultAsync(a => a.Slug == slug, cancellationToken);
        if (agency is null)
        {
            return RegisterWithActivationOutcome.AgencyNotFound;
        }

        var exists = await _db.Users.AnyAsync(
            u => u.AgencyId == agency.Id && (u.Email.ToLower() == email || u.Username.ToLower() == email),
            cancellationToken);
        if (exists)
        {
            return RegisterWithActivationOutcome.DuplicateEmail;
        }

        var isFirstUser = !await _db.Users.AnyAsync(u => u.AgencyId == agency.Id, cancellationToken);
        var (passwordHash, passwordSalt) = _authRepository.CreatePasswordHash(request.Password);
        var now = DateTimeOffset.UtcNow;

        var user = new CrmUser
        {
            Id = Guid.NewGuid(),
            AgencyId = agency.Id,
            Username = email,
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            ContactNo = request.ContactNumber?.Trim(),
            Activated = isFirstUser,
            IsSuperAdmin = isFirstUser,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Users.Add(user);

        if (isFirstUser)
        {
            var location = await _db.Locations
                .Where(l => l.AgencyId == agency.Id)
                .OrderBy(l => l.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (location is not null)
            {
                _db.UserLocations.Add(new UserLocation
                {
                    UserId = user.Id,
                    LocationId = location.Id,
                    Role = Model.Roles.Owner,
                    CreatedAt = now
                });
            }
        }

        var rawToken = TokenHasher.CreateRawToken();
        _db.AccountActivationTokens.Add(new AccountActivationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(rawToken),
            ExpiresAt = now.AddDays(2),
            CreatedAt = now
        });

        await _db.SaveChangesAsync(cancellationToken);

        if (!isFirstUser)
        {
            var frontendUrl = (_configuration["App:FrontendUrl"] ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                throw new InvalidOperationException("App:FrontendUrl is not configured.");
            }

            var activationLink = $"{frontendUrl}/activate?token={Uri.EscapeDataString(rawToken)}";
            try
            {
                await _emailSender.SendAccountActivationEmailAsync(email, activationLink, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send activation email to {Email}.", email);
                _db.AccountActivationTokens.RemoveRange(
                    _db.AccountActivationTokens.Where(t => t.UserId == user.Id));
                _db.Users.Remove(user);
                await _db.SaveChangesAsync(cancellationToken);
                return RegisterWithActivationOutcome.ActivationEmailSendFailed;
            }
        }

        return RegisterWithActivationOutcome.Success;
    }

    public async Task<ActivateAccountOutcome> CompleteAccountActivationAsync(
        string? token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ActivateAccountOutcome.InvalidOrExpiredToken;
        }

        var hash = TokenHasher.Hash(token);
        var row = await _db.AccountActivationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (row is null || row.UsedAt is not null || row.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return ActivateAccountOutcome.InvalidOrExpiredToken;
        }

        row.UsedAt = DateTimeOffset.UtcNow;
        row.User.Activated = true;
        row.User.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return ActivateAccountOutcome.Success;
    }
}
