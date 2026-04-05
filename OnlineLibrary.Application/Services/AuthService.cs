using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;
using OnlineLibrary.Domain.Enums;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="AuthService"/>.
/// </summary>
public class AuthService(
    IUserRepository userRepository,
    IUserService userService,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(string fullName, string email, string password, CancellationToken cancellationToken = default)
    {
        var emailTaken = await userRepository.ExistAsync(u => u.Email == email, cancellationToken);
        if (emailTaken)
            throw new ArgumentException($"Email '{email}' is already registered.");

        var model = new UserModel { FullName = fullName, Email = email, Role = UserRole.Member };
        var created = await userService.CreateAsync(model, password, cancellationToken);

        var token = tokenService.GenerateAccessToken(created);
        return new AuthResult(token, created);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var userModel = user.ToModel();
        var token = tokenService.GenerateAccessToken(userModel);
        return new AuthResult(token, userModel);
    }
}
