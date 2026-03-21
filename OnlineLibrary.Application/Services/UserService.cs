using System.Security.Cryptography;
using System.Text;
using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="UserService"/>.
/// </summary>
public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserModel> CreateAsync(UserModel userModel)
    {
        if (string.IsNullOrWhiteSpace(userModel.Password))
            throw new ArgumentException("Password is required when creating a user.", nameof(userModel));

        userModel.PasswordHash = HashPassword(userModel.Password);
        userModel.Password = null;

        var user = userModel.ToEntity();
        user = await userRepository.InsertAsync(user);

        await userRepository.SaveChangesAsync();

        return user.ToModel();
    }

    public async Task UpdateAsync(UserModel userModel)
    {
        var existingUser = await userRepository.GetByIdAsync(userModel.Id);
        if (existingUser is null)
            throw new KeyNotFoundException($"User with id {userModel.Id} not found");

        if (!string.IsNullOrWhiteSpace(userModel.Password))
        {
            userModel.PasswordHash = HashPassword(userModel.Password);
        }
        else
        {
            userModel.PasswordHash = existingUser.PasswordHash;
        }

        userModel.Password = null;

        var user = userModel.ToEntity();
        userRepository.Update(user);

        await userRepository.SaveChangesAsync();
    }

    public async Task<List<UserModel>> GetAsync()
    {
        var users = await userRepository.GetAllAsync();
        var userModels = users.Select(u => u.ToModel()).ToList();

        return userModels;
    }

    public async Task<UserModel> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found");

        var userModel = user.ToModel();

        return userModel;
    }

    private static string HashPassword(string password)
    {
        // Derive a PBKDF2 hash and store as: {iterations}.{salt}.{hash}, all Base64-encoded.
        const int iterations = 100_000;
        const int saltSize = 16; // 128-bit salt
        const int keySize = 32;  // 256-bit key

        var salt = RandomNumberGenerator.GetBytes(saltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);

        return $"{iterations}.{saltBase64}.{hashBase64}";
    }
}
