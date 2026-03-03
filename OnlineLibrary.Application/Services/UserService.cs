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
    public async Task<UserModel> CreateAsync(UserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Password))
            throw new ArgumentException("Password is required when creating a user.", nameof(model));

        model.PasswordHash = HashPassword(model.Password);
        model.Password = null;

        var entity = model.ToEntity();
        entity = await userRepository.InsertAsync(entity);

        await userRepository.SaveChangesAsync();

        return entity.ToModel();
    }

    public async Task UpdateAsync(UserModel model)
    {
        var existingEntity = await userRepository.GetByIdAsync(model.Id);
        if (existingEntity is null)
            throw new KeyNotFoundException($"User with id {model.Id} not found");

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            model.PasswordHash = HashPassword(model.Password);
        }
        else
        {
            model.PasswordHash = existingEntity.PasswordHash;
        }

        model.Password = null;

        var entity = model.ToEntity();
        userRepository.Update(entity);

        await userRepository.SaveChangesAsync();
    }

    public async Task<List<UserModel>> GetAsync()
    {
        var entities = await userRepository.GetAllAsync();
        var models = entities.Select(e => e.ToModel()).ToList();

        return models;
    }

    public async Task<UserModel> GetByIdAsync(int id)
    {
        var entity = await userRepository.GetByIdAsync(id);
        if (entity is null)
            throw new KeyNotFoundException($"User with id {id} not found");

        var model = entity.ToModel();

        return model;
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