using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dmb.Crm.Data.Context;
using Dmb.Crm.Data.Entities;
using Dmb.Crm.Data.Repository.Interface.Auth;
using Dmb.Crm.Model;
using Dmb.Crm.Model.Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dmb.Crm.Data.Repository.Implementation.Auth;

public class AuthRepository : IAuthRepository
{
    private readonly CrmContext _db;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public AuthRepository(CrmContext db, IMapper mapper, IConfiguration configuration, IMemoryCache cache)
    {
        _db = db;
        _mapper = mapper;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<AuthTokenLoginResult> LoginAndIssueJwtAsync(LoginDto model, CancellationToken cancellationToken = default)
    {
        var normalized = model.Username.Trim().ToLowerInvariant();
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Username.ToLower() == normalized || u.Email.ToLower() == normalized,
                cancellationToken);

        if (user is null || !VerifyPassword(model.Password, user.PasswordSalt, user.PasswordHash))
        {
            return new AuthTokenLoginResult { Status = AuthTokenLoginStatus.InvalidCredentials };
        }

        if (!user.Activated)
        {
            return new AuthTokenLoginResult
            {
                Status = AuthTokenLoginStatus.AccountBlocked,
                BlockReason = "Your account is not activated yet. Use the activation link we emailed you."
            };
        }

        var locations = await GetUserLocationsAsync(user.Id, cancellationToken);
        var loginUser = _mapper.Map<LoggedInUserDto>(user);
        return new AuthTokenLoginResult
        {
            Status = AuthTokenLoginStatus.Success,
            AccessToken = CreateAccessToken(loginUser),
            Locations = locations,
            CurrentLocationId = locations.FirstOrDefault()?.LocationId,
            FirstName = user.FirstName,
            IsSuperAdmin = user.IsSuperAdmin
        };
    }

    public async Task<LoggedInUserDto?> GetLoggedInUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        return user is null ? null : _mapper.Map<LoggedInUserDto>(user);
    }

    public async Task<(LoggedInUserDto? User, string? Error)> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return (null, "Account not found.");
        }

        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
        {
            return (null, "First name, last name, and email are required.");
        }

        var emailTaken = await _db.Users.AnyAsync(
            u => u.Id != userId && (u.Email.ToLower() == email || u.Username.ToLower() == email),
            cancellationToken);
        if (emailTaken)
        {
            return (null, "That email is already in use.");
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                !VerifyPassword(dto.CurrentPassword, user.PasswordSalt, user.PasswordHash))
            {
                return (null, "Current password is incorrect.");
            }

            var (hash, salt) = CreatePasswordHash(dto.NewPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.Username = email;
        user.ContactNo = string.IsNullOrWhiteSpace(dto.ContactNo) ? null : dto.ContactNo.Trim();
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return (_mapper.Map<LoggedInUserDto>(user), null);
    }

    public async Task<LogoutWorkflowResult> LogoutAsync(
        string? username,
        string? jti,
        string? expClaim,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(jti))
        {
            var jwtExpiresAt = DateTimeOffset.UtcNow.AddHours(1);
            if (long.TryParse(expClaim, out var expSeconds))
            {
                jwtExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
            }

            var ttl = jwtExpiresAt - DateTimeOffset.UtcNow;
            if (ttl < TimeSpan.Zero)
            {
                ttl = TimeSpan.FromMinutes(5);
            }

            _cache.Set($"revoked_jti:{jti}", true, ttl);
            await RevokeJtiAsync(jti, jwtExpiresAt, userId, cancellationToken);
        }

        return new LogoutWorkflowResult { Username = username };
    }

    public Task<bool> IsJtiRevokedAsync(string jti, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue($"revoked_jti:{jti}", out bool revoked) && revoked)
        {
            return Task.FromResult(true);
        }

        return _db.RevokedTokens.AsNoTracking().AnyAsync(t => t.Jti == jti, cancellationToken);
    }

    public async Task RevokeJtiAsync(string jti, DateTimeOffset expiresAt, Guid? userId, CancellationToken cancellationToken = default)
    {
        if (await _db.RevokedTokens.AnyAsync(t => t.Jti == jti, cancellationToken))
        {
            return;
        }

        _db.RevokedTokens.Add(new RevokedToken
        {
            Id = Guid.NewGuid(),
            Jti = jti,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);
    }

    public (string PasswordHash, string PasswordSalt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var passwordSalt = Convert.ToBase64String(hmac.Key);
        var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return (passwordHash, passwordSalt);
    }

    public bool VerifyPassword(string password, string passwordSalt, string passwordHash)
    {
        var saltBytes = Convert.FromBase64String(passwordSalt);
        using var hmac = new HMACSHA512(saltBytes);
        var computed = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computed),
            Convert.FromBase64String(passwordHash));
    }

    public async Task<IReadOnlyList<LocationMembershipDto>> GetUserLocationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user?.IsSuperAdmin == true)
        {
            return await _db.Locations.AsNoTracking()
                .Where(l => l.AgencyId == user.AgencyId && l.IsActive)
                .Select(l => new LocationMembershipDto
                {
                    LocationId = l.Id,
                    Name = l.Name,
                    Role = Roles.Owner
                })
                .ToListAsync(cancellationToken);
        }

        return await _db.UserLocations
            .AsNoTracking()
            .Where(ul => ul.UserId == userId && ul.Location.IsActive)
            .Select(ul => new LocationMembershipDto
            {
                LocationId = ul.LocationId,
                Name = ul.Location.Name,
                Role = ul.Role
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AuthTokenLoginResult> IssueJwtForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return new AuthTokenLoginResult { Status = AuthTokenLoginStatus.InvalidCredentials };
        }

        if (!user.Activated)
        {
            return new AuthTokenLoginResult
            {
                Status = AuthTokenLoginStatus.AccountBlocked,
                BlockReason = "Your account is not activated yet."
            };
        }

        var locations = await GetUserLocationsAsync(user.Id, cancellationToken);
        var loginUser = _mapper.Map<LoggedInUserDto>(user);
        return new AuthTokenLoginResult
        {
            Status = AuthTokenLoginStatus.Success,
            AccessToken = CreateAccessToken(loginUser),
            Locations = locations,
            CurrentLocationId = locations.FirstOrDefault()?.LocationId,
            FirstName = user.FirstName,
            IsSuperAdmin = user.IsSuperAdmin
        };
    }

    private string CreateAccessToken(LoggedInUserDto user)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "dmbcrm";
        var audience = _configuration["Jwt:Audience"] ?? "dmbcrm";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new Claim("agencyId", user.AgencyId.ToString()),
            new Claim("isSuperAdmin", user.IsSuperAdmin ? "true" : "false")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<IReadOnlyList<AdminUserDto>> ListAgencyUsersAsync(Guid agencyId, Guid locationId, CancellationToken cancellationToken = default)
    {
        var memberships = await _db.UserLocations.AsNoTracking()
            .Where(ul => ul.LocationId == locationId)
            .ToDictionaryAsync(ul => ul.UserId, ul => ul.Role, cancellationToken);

        var users = await _db.Users.AsNoTracking()
            .Where(u => u.AgencyId == agencyId)
            .OrderByDescending(u => u.IsSuperAdmin)
            .ThenBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return users.Select(u => new AdminUserDto
        {
            UserId = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            ContactNo = u.ContactNo,
            Activated = u.Activated,
            IsSuperAdmin = u.IsSuperAdmin,
            Role = memberships.TryGetValue(u.Id, out var role) ? role : (u.IsSuperAdmin ? Roles.Owner : Roles.User)
        }).ToList();
    }

    public async Task<AdminUserDto?> UpdateAgencyUserAsync(
        Guid agencyId,
        Guid locationId,
        Guid actorUserId,
        bool actorIsSuperAdmin,
        Guid userId,
        UpdateAdminUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.AgencyId == agencyId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (user.IsSuperAdmin && actorUserId != userId && !actorIsSuperAdmin)
        {
            return null;
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        var role = dto.Role.Trim().ToLowerInvariant();
        if (!Roles.All.Contains(role))
        {
            role = Roles.User;
        }

        var emailTaken = await _db.Users.AnyAsync(
            u => u.Id != userId && (u.Email.ToLower() == email || u.Username.ToLower() == email),
            cancellationToken);
        if (emailTaken)
        {
            throw new InvalidOperationException("That email is already in use.");
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = email;
        user.Username = email;
        user.ContactNo = string.IsNullOrWhiteSpace(dto.ContactNo) ? null : dto.ContactNo.Trim();
        user.Activated = dto.Activated;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var membership = await _db.UserLocations.FirstOrDefaultAsync(ul => ul.UserId == userId && ul.LocationId == locationId, cancellationToken);
        if (membership is null)
        {
            _db.UserLocations.Add(new UserLocation
            {
                UserId = userId,
                LocationId = locationId,
                Role = role,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            membership.Role = role;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new AdminUserDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ContactNo = user.ContactNo,
            Role = role,
            Activated = user.Activated,
            IsSuperAdmin = user.IsSuperAdmin
        };
    }

    public async Task<string?> DeleteAgencyUserAsync(Guid agencyId, Guid actorUserId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (actorUserId == userId)
        {
            return "You cannot delete your own account here.";
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && u.AgencyId == agencyId, cancellationToken);
        if (user is null)
        {
            return "User not found.";
        }

        if (user.IsSuperAdmin)
        {
            return "Super admin accounts cannot be deleted.";
        }

        _db.UserLocations.RemoveRange(_db.UserLocations.Where(ul => ul.UserId == userId));
        _db.ExternalLogins.RemoveRange(_db.ExternalLogins.Where(l => l.UserId == userId));
        _db.AccountActivationTokens.RemoveRange(_db.AccountActivationTokens.Where(t => t.UserId == userId));
        _db.PasswordResetTokens.RemoveRange(_db.PasswordResetTokens.Where(t => t.UserId == userId));
        try
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync(cancellationToken);
            return null;
        }
        catch (DbUpdateException)
        {
            _db.ChangeTracker.Clear();
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (existing is null)
            {
                return "User not found.";
            }

            existing.Activated = false;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            return "This user still has CRM records, so the account was deactivated instead of deleted.";
        }
    }
}
