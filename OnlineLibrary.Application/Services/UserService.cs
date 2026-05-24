using OnlineLibrary.Application.Abstractions.Repositories;
using OnlineLibrary.Application.Abstractions.Services;
using OnlineLibrary.Domain.Entities;
using OnlineLibrary.Domain.Enums;
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
        var existingUser = await userRepository.GetByIdAsync(userModel.Id, cancellationToken, tracked: true);
        if (existingUser is null)
            throw new KeyNotFoundException($"User with id {userModel.Id} not found");

        existingUser.FullName = userModel.FullName;
        existingUser.Email = userModel.Email;
        existingUser.Role = userModel.Role;

        await userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetByIdAsync(id, cancellationToken, tracked: true);
        if (existingUser is null)
            throw new KeyNotFoundException($"User with id {id} not found");

        existingUser.PasswordHash = passwordHasher.Hash(newPassword);

        await userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedList<UserModel>> GetAsync(int page, int pageSize, string? orderBy = null, OrderType orderType = OrderType.Asc, CancellationToken cancellationToken = default)
    {
        var paged = await userRepository.GetPagedAsync(page, pageSize, BuildOrderBy(orderBy, orderType), cancellationToken);

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

    private static Func<IQueryable<User>, IOrderedQueryable<User>> BuildOrderBy(string? orderBy, OrderType orderType) =>
        orderBy?.ToLower() switch
        {
            "email" => orderType == OrderType.Desc
                ? q => q.OrderByDescending(u => u.Email)
                : q => q.OrderBy(u => u.Email),
            "role" => orderType == OrderType.Desc
                ? q => q.OrderByDescending(u => u.Role)
                : q => q.OrderBy(u => u.Role),
            "created" => orderType == OrderType.Desc
                ? q => q.OrderByDescending(u => u.CreatedAt)
                : q => q.OrderBy(u => u.CreatedAt),
            _ => orderType == OrderType.Desc
                ? q => q.OrderByDescending(u => u.FullName)
                : q => q.OrderBy(u => u.FullName)
        };
}
