using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Models;

namespace OnlineLibrary.Application.Services;

/// <summary>
/// Represents a <see cref="UserService"/>.
/// </summary>
public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<UserModel> CreateAsync(UserModel userModel, string password, CancellationToken cancellationToken = default)
    {
        userModel.PasswordHash = passwordHasher.Hash(password);

        var user = userModel.ToEntity();
        user = await userRepository.InsertAsync(user, cancellationToken);

        await userRepository.SaveChangesAsync(cancellationToken);

        return user.ToModel();
    }

    public async Task UpdateAsync(UserModel userModel, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetByIdAsync(userModel.Id, cancellationToken);
        if (existingUser is null)
            throw new KeyNotFoundException($"User with id {userModel.Id} not found");

        userModel.PasswordHash = existingUser.PasswordHash;

        var user = userModel.ToEntity();
        userRepository.Update(user);

        await userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetByIdAsync(id, cancellationToken);
        if (existingUser is null)
            throw new KeyNotFoundException($"User with id {id} not found");

        existingUser.PasswordHash = passwordHasher.Hash(newPassword);
        userRepository.Update(existingUser);

        await userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedList<UserModel>> GetAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var paged = await userRepository.GetPagedAsync(page, pageSize, cancellationToken);

        return new PagedList<UserModel>
        {
            Items = paged.Items.Select(u => u.ToModel()).ToList(),
            TotalCount = paged.TotalCount,
            CurrentPage = paged.CurrentPage,
            PageSize = paged.PageSize
        };
    }

    public async Task<UserModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found");

        var userModel = user.ToModel();

        return userModel;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found");

        userRepository.Delete(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
