using AutoMapper;
using PortfolioSaaS.Application.DTOs.Auth;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Domain.Enums;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;
using static PortfolioSaaS.Infrastructure.Specifications.UserSpecs;

namespace PortfolioSaaS.Infrastructure.Services;

public class AuthService(BaseRepository<User> userRepository, JwtTokenService jwtTokenService)
{
    private readonly BaseRepository<User> _userRepository = userRepository;
    private readonly JwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FirstOrDefaultBySpecAsync(UserSpecs.ByEmail(request.Email), cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return BuildResponse(user);
    }

    public async Task<LoginResponse?> RegisterTenantAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _userRepository.CountAsync(UserSpecs.ByEmail(request.Email), cancellationToken);
        var emailExists = count > 0;

        if (emailExists)
            return null;

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            ConfiguredDomain = request.ConfiguredDomain
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.TenantOwner
        };

        if (_userRepository.GetType().GetField("_db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_userRepository) is not ApplicationDbContext db) throw new InvalidOperationException("Could not get database context");

        db.Tenants.Add(tenant);
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return BuildResponse(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.SaveAsync(user, cancellationToken);
        return true;
    }

    private LoginResponse BuildResponse(User user)
    {
        var response = new LoginResponse
        {
            Token = _jwtTokenService.GenerateToken(user),
            Email = user.Email,
            Role = user.Role
        };
        return response;
    }
}
