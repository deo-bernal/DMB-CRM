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

public class PasswordResetRepository : IPasswordResetRepository
{
    private readonly CrmContext _db;
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordResetEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordResetRepository> _logger;

    public PasswordResetRepository(
        CrmContext db,
        IAuthRepository authRepository,
        IPasswordResetEmailSender emailSender,
        IConfiguration configuration,
        ILogger<PasswordResetRepository> logger)
    {
        _db = db;
        _authRepository = authRepository;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ForgotPasswordRequestStatus> RequestPasswordResetAsync(
        ForgotPasswordDto request,
        CancellationToken cancellationToken = default)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, cancellationToken);
        if (user is null)
        {
            return ForgotPasswordRequestStatus.Ok;
        }

        var existing = await _db.PasswordResetTokens.Where(t => t.UserId == user.Id).ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            _db.PasswordResetTokens.RemoveRange(existing);
        }

        var rawToken = TokenHasher.CreateRawToken();
        var row = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(rawToken),
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.PasswordResetTokens.Add(row);
        await _db.SaveChangesAsync(cancellationToken);

        var frontendUrl = (_configuration["App:FrontendUrl"] ?? string.Empty).TrimEnd('/');
        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            throw new InvalidOperationException("App:FrontendUrl is not configured.");
        }

        var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";
        try
        {
            await _emailSender.SendPasswordResetEmailAsync(user.Email, resetLink, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}.", user.Email);
            _db.PasswordResetTokens.Remove(row);
            await _db.SaveChangesAsync(cancellationToken);
            return ForgotPasswordRequestStatus.EmailServiceUnavailable;
        }

        return ForgotPasswordRequestStatus.Ok;
    }

    public async Task<PasswordResetCompletionStatus> CompletePasswordResetAsync(
        ResetPasswordDto request,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return PasswordResetCompletionStatus.PasswordMismatch;
        }

        var hash = TokenHasher.Hash(request.Token);
        var row = await _db.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (row is null || row.UsedAt is not null || row.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return PasswordResetCompletionStatus.InvalidOrExpiredToken;
        }

        var (passwordHash, passwordSalt) = _authRepository.CreatePasswordHash(request.NewPassword);
        row.User.PasswordHash = passwordHash;
        row.User.PasswordSalt = passwordSalt;
        row.User.UpdatedAt = DateTimeOffset.UtcNow;
        row.UsedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return PasswordResetCompletionStatus.Success;
    }
}
